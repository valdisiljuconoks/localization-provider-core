// Copyright (c) 2018 Valdis Iljuconoks.
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
using DbLocalizationProvider.AspNetCore.Commands;
using DbLocalizationProvider.Commands;
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
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProviderAdminUI(this IApplicationBuilder app, string path = "/localization-admin")
        {
            if(path == null)
                throw new ArgumentNullException(nameof(path));

            app.Map(new PathString(path),
                    builder =>
                    {
                        builder.UseFileServer(new FileServerOptions
                                              {
                                                  FileProvider = new EmbeddedFileProvider(typeof(IApplicationBuilderExtensions).Assembly),
                                                  EnableDefaultFiles = true,
                                                  DefaultFilesOptions = { DefaultFileNames = new[] { "adminui.html" } }
                                              });

                        var routeBuilder = new RouteBuilder(builder)
                                           {
                                               DefaultHandler = app.ApplicationServices.GetRequiredService<MvcRouteHandler>()
                                           };

                        routeBuilder.MapRoute("Admin UI webapi route", "api/{controller=Service}/{action=Index}");
                        var route = routeBuilder.Build();
                        builder.UseRouter(route);
                    });

            // we need to set handlers at this stage as Mvc config might be added to the service collection *after* DbLocalizationProvider
            var factory = ConfigurationContext.Current.TypeFactory;
            factory.ForQuery<AvailableLanguages.Query>()
                   .SetHandler(() => new AvailableLanguagesHandler(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()));

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();

            //ConfigurationContext.Current.TypeFactory.ForQuery<GetAllTranslations.Query>().SetHandler<GetAllTranslations.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResource.Command>().SetHandler<CreateNewResource.Handler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResource.Handler>();

            return app;
        }
    }
}
