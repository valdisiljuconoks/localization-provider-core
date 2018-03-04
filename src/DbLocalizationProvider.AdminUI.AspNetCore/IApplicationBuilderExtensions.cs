using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProviderAdminUI(this IApplicationBuilder app, string path)
        {

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

                        routeBuilder.MapRoute("Admin UI webapi route", "api/{controller=AdminUI}/{action=Index}");
                        var route = routeBuilder.Build();
                        builder.UseRouter(route);
                    });


            return app;
        }
    }
}
