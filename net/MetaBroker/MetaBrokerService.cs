using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MetaThrift;
using Thrift;

namespace MetaBroker
{
    class MetaBrokerService : MetaThrift.MetaBroker.Iface
    {
        private readonly string _name;
        private readonly string _displayName;
        private readonly string _description;

        private class ServiceContext
        {
            public MetaServiceInfo Info { get; private set; }
            public MetaService.Client Client { get; private set; }

            public ServiceContext(MetaServiceInfo serviceInfo)
            {
                Info = serviceInfo;
                Client = new MetaService.Client(serviceInfo.Url.CreateProtocol());
            }
        }

        private readonly ConcurrentDictionary<string, ServiceContext> _services = new ConcurrentDictionary<string, ServiceContext>();

        public MetaBrokerService(string name, string displayName = null, string description = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            _name = name;
            _displayName = displayName ?? _name;
            _description = description ?? String.Empty;
        }

        public MetaBrokerService() : this("MetaBroker")
        {}

        public void bind(MetaServiceInfo serviceInfo)
        {
            HandledCall(info =>
                {
                    var serviceName = info.Name;
                    var serviceContext = new ServiceContext(info);
                    _services[serviceName] = serviceContext;
                }, serviceInfo, "Could not bind service");
        }

        public void unbind(string serviceName)
        {
            ServiceContext context;
            HandledCall(name => _services.TryRemove(name, out context), serviceName, "Could not unbind service");
        }

        public MetaServiceInfo getInfo(string serviceName)
        {
            return HandledCall(() => _services[serviceName].Info, "Could not get service");
        }

        public List<MetaServiceInfo> getInfos()
        {
            return HandledCall(() => _services.Values.Select(c => c.Info).ToList(), "Could not get services");
        }

        public string getName() { return _name; }
        public string getDisplayName() { return _displayName; }
        public string getDescription() { return _description; }
        public void ping() {}

        public List<MetaOperation> getOperations()
        {
            return HandledCall(() => _services.Values.
                SelectMany(serviceContext => serviceContext.Client.getOperations().
                    Select(operation => operation.Wrap(serviceContext.Info.Name)))
                .ToList(), "Could not get operations");
        }

        public MetaObject call(MetaOperation operation, MetaObject input)
        {
            return HandledCall((o, i) => _services[o.GetServiceName()].Client.call(o.Unwrap(), i), operation, input, "Could not call operation");
        }

        private static void HandledCall<TInput>(Action<TInput> action, TInput input, string couldNot)
        {
            try
            {
                action(input);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}: {1}", couldNot, ex);
                throw new ServiceException {Reason = String.Format("{0}: {1}", couldNot, ex.Message)};
            }
        }

        private static TOutput HandledCall<TOutput>(Func<TOutput> func, string couldNot)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}: {1}", couldNot, ex);
                throw new ServiceException {Reason = String.Format("{0}: {1}", couldNot, ex.Message)};
            }
        }

        private static TOutput HandledCall<TInput1, TInput2, TOutput>(Func<TInput1, TInput2, TOutput> func,
                                                                      TInput1 input1, TInput2 input2, string couldNot)
        {
            try
            {
                return func(input1, input2);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}: {1}", couldNot, ex);
                throw new ServiceException {Reason = String.Format("{0}: {1}", couldNot, ex.Message)};
            }
        }
    }
}