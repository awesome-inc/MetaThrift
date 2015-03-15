using System.Collections.Generic;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.Models.Services
{
    public interface IMetaServiceProvider
    {
        string Uri { get; set; }
        IEnumerable<IMetaService> Refresh();
    }
}
