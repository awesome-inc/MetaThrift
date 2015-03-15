using System;
using MetaBrowser.Models.Entities;
using MetaThrift;
using MetaOperation = MetaBrowser.Models.Entities.MetaOperation;
using MetaObject = MetaBrowser.Models.Entities.MetaObject;

namespace MetaBrowser.Models.Services.Thrift
{
    class ThriftMetaService : IMetaService
    {
        private readonly MetaService.Iface _service;

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public MetaOperation[] Operations { get; set; }

        public ThriftMetaService(MetaService.Iface service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
        }

        public MetaObject Call(MetaOperation operation, MetaObject input)
        {
            return FromMeta(_service.call(ToMeta(operation), ToMeta(input)));
        }

        private static MetaThrift.MetaObject ToMeta(MetaObject input)
        {
            return new MetaThrift.MetaObject {TypeName = input.Type, Data = input.Data};
        }

        private static MetaObject FromMeta(MetaThrift.MetaObject input)
        {
            return new MetaObject { Type = input.TypeName, Data = input.Data };
        }

        private static MetaThrift.MetaOperation ToMeta(MetaOperation input)
        {
            return new MetaThrift.MetaOperation
            {
                Name = input.Name,
                InputTypeName = input.InputType,
                OutputTypeName = input.OutputType,
                Description = input.Description,
                DisplayName = input.DisplayName
            };
        }

    }
}