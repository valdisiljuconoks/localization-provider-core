using System.Collections.Generic;
using DbLocalizationProvider.Export;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Configuration
{
    public class ProviderSettings
    {
        public List<IResourceExporter> Exporters { get; set; } = new List<IResourceExporter>();
    }
}
