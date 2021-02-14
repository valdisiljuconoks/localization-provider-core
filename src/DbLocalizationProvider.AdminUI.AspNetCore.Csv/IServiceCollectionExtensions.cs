// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AdminUI.AspNetCore.Configuration;
using DbLocalizationProvider.Csv;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Csv
{
    /// <summary>
    /// Do I really need to document extension classes?
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        ///Adds support for export and import in Csv format
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        public static IServiceCollection AddCsvFormat(
            this IServiceCollection services)
        {
            services.Configure<ProviderSettings>(s => s.Exporters.Add(new CsvResourceExporter()));

            return services;
        }
    }
}
