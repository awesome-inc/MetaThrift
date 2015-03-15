using System.IO;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.Models.Services
{
    public interface IMetaServiceExporter
    {
        ExporterInfo Info { get; }
        void Export(IMetaService service, Stream stream);
    }
}