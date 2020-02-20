// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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

        public static IApplicationBuilder UseDbLocalizationClientsideProvider(
            this IApplicationBuilder builder,
            string path = "jsl10n")
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            ClientsideConfigurationContext.SetRootPath(path);
            AppContext.Configure(builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            _cache = builder.ApplicationServices.GetRequiredService<ICacheManager>();

            var cacheManager = builder.ApplicationServices.GetService<ICacheManager>();
            cacheManager.OnRemove += OnOnRemove;
            builder.ApplicationServices.GetRequiredService<IApplicationLifetime>()
                .ApplicationStopping
                .Register(() => cacheManager.OnRemove -= OnOnRemove);

            builder.MapWhen(context => context.Request.Path.StartsWithSegments("/" + ClientsideConfigurationContext.RootPath),
                            _ => { _.UseMiddleware<RequestHandler>(); });

            return builder;
        }

        private static void OnOnRemove(CacheEventArgs args)
        {
            CacheHelper.CacheManagerOnOnRemove(args, _cache);
        }
    }
}
