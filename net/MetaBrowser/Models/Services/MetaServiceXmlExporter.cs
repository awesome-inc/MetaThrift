using System.IO;
using System.Linq;
using System.Xml.Linq;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.Models.Services
{
    class MetaServiceXmlExporter : IMetaServiceExporter
    {
        private static readonly ExporterInfo ExporterInfo = new ExporterInfo
            {
                Name = "MetaService Xml Exporter", 
                Description = "Simple MetaService Xml Exporter",
                Format = "xml",
                MimeType = "application/xml"
            };

        public string Header
        {
            get { return "XML"; }
        }

        public ExporterInfo Info { get { return ExporterInfo; } }

        public void Export(IMetaService service, Stream stream)
        {
            new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("MetaService Xml Export"),
                new XElement("Service",
                             new XAttribute("Name", service.Name),
                             new XElement("DisplayName", service.DisplayName),
                             new XElement("Description", service.Description),
                             new XElement("Operations", (service.Operations == null || service.Operations.Length == 0)
                                 ? null 
                                 : service.Operations.Select(f =>
                                     new XElement("Operation",
                                         new XAttribute("Name", f.Name),
                                         new XElement("InputType", f.InputType),
                                         new XElement("OutputType", f.OutputType),
                                         new XElement("DisplayName", f.DisplayName),
                                         new XElement("Description", f.Description))
                                     )
                             )
                    )
                )
                .Save(stream);
        }
    }
}