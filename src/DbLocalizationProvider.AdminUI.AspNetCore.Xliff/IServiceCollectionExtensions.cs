// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AdminUI.AspNetCore.Configuration;
using DbLocalizationProvider.Xliff;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    /// <summary>
    /// Do I really need to document extension classes?
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        ///Adds support for export and import in Xliff format
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        public static IServiceCollection AddXliffFormat(
            this IServiceCollection services)
        {
            services.Configure<ProviderSettings>(s => s.Exporters.Add(new XliffResourceExporter()));

            return services;
        }
    }
}
