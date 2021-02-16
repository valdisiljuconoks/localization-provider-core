// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AdminUI.AspNetCore.Configuration;
using DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using AvailableLanguagesHandler = DbLocalizationProvider.AdminUI.AspNetCore.Queries.AvailableLanguagesHandler;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    /// <summary>
    /// Do I really need to document extension classes? (Making analyzer happy)
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Use this method if you wanna see AdminUI under given path.
        /// </summary>
        /// <param name="app">Whatever</param>
        /// <returns>If you want to chain calls further, you can use the same application builder that was used.</returns>
        public static IApplicationBuilder UseDbLocalizationProviderAdminUI(this IApplicationBuilder app)
        {
            var path = app.ApplicationServices.GetService<UiConfigurationContext>().RootUrl;
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            // add checker middleware - to support registration order verification
            app.UseMiddleware<AdminUIMarkerMiddleware>();

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new EmbeddedFileProvider(typeof(IApplicationBuilderExtensions).Assembly),
            //    ServeUnknownFileTypes = true,
            //    RequestPath = path + "/res"
            //});

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new EmbeddedFileProvider(
                    typeof(IApplicationBuilderExtensions).Assembly,
                    "DbLocalizationProvider.AdminUI.AspNetCore.node_modules._fortawesome.fontawesome_free.webfonts"),
                ServeUnknownFileTypes = true,
                RequestPath = path + "/webfonts"
            });

            var factory = app.ApplicationServices.GetService<TypeFactory>();
            var requestOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();

            // we need to override default handler at this stage
            // as Mvc config might be added to the service collection *after* DbLocalizationProvider
            factory
                .ForQuery<AvailableLanguages.Query>()
                .SetHandler(() => new AvailableLanguagesHandler(requestOptions.Value.SupportedUICultures));

            // postfix registered providers
            var providerSettings = app.ApplicationServices.GetService<IOptions<ProviderSettings>>();
            if (providerSettings != null)
            {
                var context = app.ApplicationServices.GetService<ConfigurationContext>();
                foreach (var exporter in providerSettings.Value.Exporters)
                {
                    context.Export.Providers.Add(exporter);
                }
            }

            return app;
        }
    }
}
