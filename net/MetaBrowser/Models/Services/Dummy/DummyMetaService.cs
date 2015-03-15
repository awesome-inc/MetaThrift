using System;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.Models.Services.Dummy
{
    class DummyMetaService : IMetaService
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public MetaOperation[] Operations { get; set; }

        public MetaObject Call(MetaOperation operation, MetaObject input)
        {
            throw new NotImplementedException("Just a dummy implementation");
        }
    }
}