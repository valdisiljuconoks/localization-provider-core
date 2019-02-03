// Copyright (c) 2019 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
