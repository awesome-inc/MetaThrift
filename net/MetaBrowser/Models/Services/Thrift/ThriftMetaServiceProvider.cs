using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MetaBrowser.Models.Entities;
using MetaThrift;
using Thrift;
using MetaOperation = MetaBrowser.Models.Entities.MetaOperation;

namespace MetaBrowser.Models.Services.Thrift
{
    [Export(typeof(IMetaServiceProvider))]
    public class ThriftMetaServiceProvider : IMetaServiceProvider
    {
        private readonly List<IMetaService> _services = new List<IMetaService>();

        public string Uri { get; set; }

        public ThriftMetaServiceProvider()
        {
            Uri = "tcp://localhost:9090";
        }

        public IEnumerable<IMetaService> Refresh()
        {
            UpdateServices();
            return _services.AsReadOnly();
        }

        private void UpdateServices()
        {
            var broker = new MetaBroker.Client(Uri.CreateProtocol());

            var operations = broker.getOperations();
            var services = broker.getInfos().Select(serviceInfo => 
                new ThriftMetaService(new MetaService.Client(serviceInfo.Url.CreateProtocol()))
                    {
                        Name = serviceInfo.Name,
                        Operations = operations.Where(f => f.GetServiceName() == serviceInfo.Name).
                            Select(f => f.Unwrap()).
                            Select(f => new MetaOperation
                            {
                                Name = f.Name,
                                InputType = f.InputTypeName,
                                OutputType = f.OutputTypeName,
                                Description = f.Description,
                                DisplayName = f.DisplayName
                            }).
                            ToArray()
                    });

            _services.Clear();
            _services.AddRange(services);
        }

    }
}
