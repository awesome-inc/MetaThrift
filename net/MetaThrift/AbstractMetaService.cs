using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MetaThrift
{
    public abstract class AbstractMetaService : MetaService.Iface
    {
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Description { get; protected set; }

        public abstract IEnumerable<MetaOperation> Operations { get; }
        public abstract object Call(MetaOperation operation, object value);

        #region Overrides

        string MetaService.Iface.getName() { return Name ?? String.Empty; }
        string MetaService.Iface.getDisplayName() { return DisplayName ?? String.Empty; }
        string MetaService.Iface.getDescription() { return Description ?? String.Empty; }

        void MetaService.Iface.ping() {}
        List<MetaOperation> MetaService.Iface.getOperations() { return Operations.ToList(); }

        MetaObject MetaService.Iface.call(MetaOperation operation, MetaObject arg)
        {
            VerifyOperation(operation);
            var input = VerifyInput(operation.InputTypeName, arg);
            var output = VerifiedCall(operation, input);
            return VerifyOutput(operation.OutputTypeName, output);
        }

        #endregion

        #region Private Methods

        private void VerifyOperation(MetaOperation operation)
        {
            if (Operations.Contains(operation)) return;

            var msg = "The specified operation is not served by this instance: "
                      + operation.PrettyPrint();
            Trace.TraceWarning(msg);
            throw new ArgumentException { Reason = msg };
        }

        private static object VerifyInput(string typeName, MetaObject value)
        {
            if (!TypeEquals(typeName, value.TypeName))
            {
                var msg = String.Format("The operation input type mismatches the type of the specified input: {0} != {1}",
                    typeName, value.TypeName);
                Trace.TraceWarning(msg);
                throw new ArgumentException { Reason = msg };
            }

            try // try accessing the input value
            {
                return value.FromMetaObject();
            }
            catch (Exception ex)
            {
                var msg = "The specified input value could not be deserialized as the specified type: " + value.TypeName;
                Trace.TraceWarning("{0}: {1}", msg, ex);
                throw new ArgumentException { Reason = msg };
            }
        }

        private static bool TypeEquals(string typeName, string otherTypeName)
        {
            return String.IsNullOrEmpty(typeName) 
                ? String.IsNullOrEmpty(otherTypeName) 
                : String.Equals(typeName, otherTypeName);
        }

        private static MetaObject VerifyOutput(string typeName, object value)
        {
            try
            {
                var type = typeName.ToType();
                return value.ToMetaObject(type);
            }
            catch (Exception ex)
            {
                var msg = "The specified output value could not be serialized to the specified type: " + typeName;
                Trace.TraceWarning("{0}: {1}", msg, ex);
                throw new ArgumentException { Reason = msg };
            }
        }


        private object VerifiedCall(MetaOperation operation, object input)
        {
            try
            {
                return Call(operation, input);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("The operation failed: {0}", ex);
                throw new ServiceException { Reason = String.Format("The operation failed: {0}", ex.Message) };
            }
        }

        #endregion
    }
}