// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AdminUI.AspNetCore.Queries;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

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
            var path = UiConfigurationContext.Current.RootUrl;

            if(path == null)
                throw new ArgumentNullException(nameof(path));

            app.Map(new PathString(path),
                    builder =>
                    {
                        builder.Map(new PathString("/res"),
                                    _ =>
                                    {
                                        _.UseFileServer(new FileServerOptions
                                                        {
                                                            FileProvider = new EmbeddedFileProvider(typeof(IApplicationBuilderExtensions).Assembly)
                                                        });
                                    });

                        builder.Map(new PathString("/webfonts"),
                                    _ =>
                                    {
                                        _.UseFileServer(new FileServerOptions
                                                        {
                                                            FileProvider = new EmbeddedFileProvider(typeof(IApplicationBuilderExtensions).Assembly, "DbLocalizationProvider.AdminUI.AspNetCore.node_modules._fortawesome.fontawesome_free.webfonts")
                                                        });
                                    });

                        var routeBuilder = new RouteBuilder(builder)
                        {
                            DefaultHandler = app.ApplicationServices.GetRequiredService<MvcRouteHandler>()
                        };

                        routeBuilder.MapRoute("Admin UI api route", "api/{controller=Service}/{action=Index}");
                        var apiRoute = routeBuilder.Build();
                        builder.UseRouter(apiRoute);
                    });

            // we need to set handlers at this stage as Mvc config might be added to the service collection *after* DbLocalizationProvider
            var factory = ConfigurationContext.Current.TypeFactory;
            factory.ForQuery<AvailableLanguages.Query>()
                   .SetHandler(() => new AvailableLanguagesHandler(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()));

            return app;
        }
    }
}
