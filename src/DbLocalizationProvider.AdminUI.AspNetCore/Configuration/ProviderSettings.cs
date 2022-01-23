// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Export;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Configuration
{
    public class ProviderSettings
    {
        public List<IResourceExporter> Exporters { get; set; } = new List<IResourceExporter>();
    }
}
