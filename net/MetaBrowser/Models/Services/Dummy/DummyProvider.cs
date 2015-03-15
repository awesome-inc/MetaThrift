using System.Collections.Generic;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.Models.Services.Dummy
{
    public class DummyProvider : IMetaServiceProvider
    {
        private readonly List<IMetaService> _services = new List<IMetaService>
            {
                new DummyMetaService
                {
                    Name = "dummyService1",
                    Operations = new []
                    {
                        new MetaOperation { Name="Increment", InputType = "int", OutputType="int", Description = "Increments the specified number by one" },
                        new MetaOperation { Name="Decrement", InputType = "int", OutputType="int", Description = "Decrements the specified number by one" },
                        new MetaOperation { Name="SayHello", Description = "Says hello to everyone" }
                    }
                },
                new DummyMetaService
                {
                    Name = "dummyService2",
                    Operations = new []
                    {
                        new MetaOperation { Name="Start", InputType="string", Description = "Starts the specified command" },
                        new MetaOperation { Name="SayHello", InputType = "string", OutputType="string", Description = "Says hello to the specified user" }
                    }
                },
            };

        public DummyProvider()
        {
            Uri = "tcp://localhost:9090";
        }

        public string Uri { get; set;  }

        IEnumerable<IMetaService> IMetaServiceProvider.Refresh()
        {
            return _services.AsReadOnly();
        }
    }
}