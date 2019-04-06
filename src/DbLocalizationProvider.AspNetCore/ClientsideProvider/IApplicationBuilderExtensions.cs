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
using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AppContext = DbLocalizationProvider.AspNetCore.ClientsideProvider.AppContext;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IApplicationBuilderClientsideExtensions
    {
        private static ICacheManager _cache;

        public static IApplicationBuilder UseDbLocalizationClientsideProvider(this IApplicationBuilder builder, string path = "jsl10n")
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            ClientsideConfigurationContext.SetRootPath(path);
            AppContext.Configure(builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            _cache = builder.ApplicationServices.GetRequiredService<ICacheManager>();

            var cacheManager = builder.ApplicationServices.GetService<ICacheManager>();
            cacheManager.OnRemove += OnOnRemove;
            builder.ApplicationServices.GetRequiredService<IApplicationLifetime>().ApplicationStopping.Register(() => cacheManager.OnRemove -= OnOnRemove);

            builder.MapWhen(context => context.Request.Path.ToString().StartsWith(ClientsideConfigurationContext.RootPath),
                            _ => { _.UseMiddleware<RequestHandler>(); });

            return builder;
        }

        private static void OnOnRemove(CacheEventArgs args)
        {
            CacheHelper.CacheManagerOnOnRemove(args, _cache);
        }
    }
}
