using System;
using System.Linq;

namespace MetaThrift
{
    public static class Extensions
    {
        #region Call

        public static void Call<TInput>(this MetaService.Iface service, string name, TInput input)
        {
            service.call(name.ToMetaAction<TInput>(), input.ToMetaObject());
        }

        public static void Call(this MetaService.Iface service, string name)
        {
            service.call(name.ToMetaAction(), MetaObject.Empty);
        }

        public static TOutput Call<TOutput>(this MetaService.Iface service, string name)
        {
            return service.call(name.ToMetaFunction<TOutput>(), MetaObject.Empty).FromMetaObject<TOutput>();
        }

        public static TOutput Call<TInput, TOutput>(this MetaService.Iface service, string name, TInput input)
        {
            return service.call(name.ToMetaFunction<TInput, TOutput>(), input.ToMetaObject()).FromMetaObject<TOutput>();
        }

        #endregion

        #region Conversion

        public static MetaOperation ToMetaAction<TInput>(this string name, 
            string displayName = null, string description = null)
        {
            var action = new MetaOperation
                {
                    Name = name,
                    InputTypeName = typeof (TInput).ToTypeName()
                };
            if (displayName != null) action.DisplayName = displayName;
            if (description != null) action.Description = description;
            return action;
        }

        public static MetaOperation ToMetaAction(this string name, 
            string displayName = null, string description = null)
        {
            var action = new MetaOperation { Name = name };
            if (displayName != null) action.DisplayName = displayName;
            if (description != null) action.Description = description;
            return action;
        }

        public static MetaOperation ToMetaFunction<TInput, TOutput>(this string name, 
            string displayName = null,string description = null)
        {
            var function = new MetaOperation
                {
                    Name = name,
                    InputTypeName = typeof (TInput).ToTypeName(),
                    OutputTypeName = typeof (TOutput).ToTypeName(),
                };
            if (displayName != null) function.DisplayName = displayName;
            if (description != null) function.Description = description;
            return function;
        }

        public static MetaOperation ToMetaFunction<TOutput>(this string name, 
            string displayName = null, string description = null)
        {
            var function = new MetaOperation
                {
                    Name = name,
                    OutputTypeName = typeof(TOutput).ToTypeName(),
                };
            if (displayName != null) function.DisplayName = displayName;
            if (description != null) function.Description = description;
            return function;
        }

        #endregion

        public static string PrettyPrint(this MetaOperation operation)
        {
            return String.Format("{2} {0}({1});{3}",
                operation.Name,
                !String.IsNullOrEmpty(operation.InputTypeName) ? operation.InputTypeName + " value" : String.Empty,
                !String.IsNullOrEmpty(operation.OutputTypeName) ? operation.OutputTypeName : "void",
                !String.IsNullOrEmpty(operation.Description) ? " // " + operation.Description : String.Empty);
        }

        public static string PrettyPrint(this MetaObject value)
        {
            return String.Format("{{type={0}, value={1}}}", value.TypeName, value.Data);
        }

        #region Broker

        public static string GetServiceName(this MetaOperation operation)
        {
            return GetServiceName(operation.Name);
        }

        public static MetaOperation Wrap(this MetaOperation operation, string serviceName)
        {
            var wrappedAction = Copy(operation);
            wrappedAction.Name = String.Concat(serviceName, "/", operation.Name);
            return wrappedAction;
        }

        public static MetaOperation Unwrap(this MetaOperation operation)
        {
            var unwrappedAction = Copy(operation);
            unwrappedAction.Name = String.Join("/", operation.Name.Split('/').Skip(1));
            return unwrappedAction;
        }

        #endregion

        #region Internal

        internal static MetaObject ToMetaObject<T>(this T value)
        {
            return value.ToMetaObject(typeof(T));
        }

        internal static MetaObject ToMetaObject(this object value, Type type)
        {
            return new MetaObject {TypeName = type.ToTypeName(), Data = value.ToJson(type) };
        }

        internal static T FromMetaObject<T>(this MetaObject value)
        {
            return value.Data.FromJson<T>();
        }

        internal static object FromMetaObject(this MetaObject value)
        {
            var type = value.TypeName.ToType();
            return type != typeof(void) ? value.Data.FromJson(type) : null;
        }

        private static string GetServiceName(string operationName)
        {
            return operationName.Split('/').First();
        }

        private static MetaOperation Copy(MetaOperation operation)
        {
            var copy = new MetaOperation { Name = operation.Name };
            // Isset!
            if (operation.InputTypeName != null) copy.InputTypeName = operation.InputTypeName;
            if (operation.OutputTypeName != null) copy.OutputTypeName = operation.OutputTypeName;
            if (operation.DisplayName != null) copy.DisplayName = operation.DisplayName;
            if (operation.Description != null) copy.Description = operation.Description;
            return copy;
        }
        
        #endregion
    }
}