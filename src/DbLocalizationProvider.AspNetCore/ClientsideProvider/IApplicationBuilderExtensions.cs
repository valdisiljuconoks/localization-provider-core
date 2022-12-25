// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppContext = DbLocalizationProvider.AspNetCore.ClientsideProvider.AppContext;

// ReSharper disable once CheckNamespace
namespace DbLocalizationProvider.AspNetCore;

public static class IApplicationBuilderClientsideExtensions
{
    private static ICacheManager _cache;

    /// <summary>
    /// Makes sure that usage of the clientside resource provider is added to your app.
    /// </summary>
    /// <param name="builder">Application builder instance passed in by framework.</param>
    /// <returns>Returns the same builder for the fluent API.</returns>
    public static IApplicationBuilder UseDbLocalizationClientsideProvider(this IApplicationBuilder builder)
    {
        AppContext.Configure(builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        _cache = builder.ApplicationServices.GetRequiredService<ICacheManager>();

        var cacheManager = builder.ApplicationServices.GetService<ICacheManager>();
        cacheManager.OnRemove += OnRemove;
        builder.ApplicationServices
            .GetRequiredService<IHostApplicationLifetime>()
            .ApplicationStopping
            .Register(() => cacheManager.OnRemove -= OnRemove);

        return builder;
    }

    private static void OnRemove(CacheEventArgs args)
    {
        CacheHelper.CacheManagerOnRemove(args, _cache);
    }
}
