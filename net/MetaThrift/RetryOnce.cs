using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MetaThrift
{
    public class RetryOnce<TException> : MetaService.Iface where TException : Exception
    {
        private const string ErrorMessage = "An error occurred: ";
        private readonly Func<MetaService.Iface> _getService;
        private MetaService.Iface _service;

        private MetaService.Iface Service
        {
            get { return _service ?? (_service = _getService()); }
            set { _service = value; }
        }

        public RetryOnce(Func<MetaService.Iface> getService)
        {
            if (getService == null) throw new ArgumentNullException("getService");
            _getService = getService;
        }

        public string getName() { return Strategy(() => Service.getName()); }
        public string getDisplayName() { return Strategy(() => Service.getDisplayName()); }
        public string getDescription() { return Strategy(() => Service.getDescription()); }
        public void ping() { Strategy(() => Service.ping()); }
        public List<MetaOperation> getOperations() { return Strategy(() => Service.getOperations()); }
        public MetaObject call(MetaOperation operation, MetaObject input) { return Strategy((o, v) => Service.call(o, v), operation, input);}

        private void Strategy(Action action)
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                Trace.TraceWarning(ErrorMessage + ex);
                Service = null;
                action();
            }
        }

        private TOutput Strategy<TOutput>(Func<TOutput> func)
        {
            try
            {
                return func();
            }
            catch (TException ex)
            {
                Trace.TraceWarning(ErrorMessage + ex);
                Service = null;
                return func();
            }
        }

        private TOutput Strategy<TInput1, TInput2, TOutput>(Func<TInput1, TInput2, TOutput> func, TInput1 input1, TInput2 input2)
        {
            try
            {
                return func(input1, input2);
            }
            catch (TException ex)
            {
                Trace.TraceWarning(ErrorMessage + ex);
                Service = null;
                return func(input1, input2);
            }
        }
    }
}