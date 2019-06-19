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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    /// <summary>
    /// Do I really need to document extension classes?
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Use this method if you want to add AdminUI component to your application. This is just a part of the setup. You will also need to mount the component. Use other method (will leave it up to you to figure out which).
        /// </summary>
        /// <param name="services">Collection of the services (Microsoft approach for DI).</param>
        /// <param name="setup">UI setup context will be passed in, so you can do some customization on that object to influence how AdminUI behaves.</param>
        /// <returns>The same service collection - so you can do chaining.</returns>
        public static IServiceCollection AddDbLocalizationProviderAdminUI(this IServiceCollection services, Action<UiConfigurationContext> setup = null)
        {
            setup?.Invoke(UiConfigurationContext.Current);

            services.AddSingleton(_ => UiConfigurationContext.Current);
            services.AddScoped<AuthorizeRolesAttribute>();

            // add support for admin ui razor class library pages
            services.Configure<RazorPagesOptions>(_ =>
                                                  {
                                                      _.Conventions.AuthorizeAreaPage("4D5A2189D188417485BF6C70546D34A1", "/AdminUI");
                                                      _.Conventions.AuthorizeAreaPage("4D5A2189D188417485BF6C70546D34A1", "/AdminUITree");

                                                      _.Conventions.AddAreaPageRoute("4D5A2189D188417485BF6C70546D34A1",
                                                                                     "/AdminUI",
                                                                                     UiConfigurationContext.Current.RootUrl);
                                                      _.Conventions.AddAreaPageRoute("4D5A2189D188417485BF6C70546D34A1",
                                                                                     "/AdminUITree",
                                                                                     UiConfigurationContext.Current.RootUrl + "/tree");
                                                  });
            return services;
        }
    }
}
