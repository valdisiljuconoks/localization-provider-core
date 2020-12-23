// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.AspNetCore.DataAnnotations;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Extension for adding localization provider services to the collection
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the database localization provider.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="setup">The setup.</param>
        /// <returns></returns>
        public static IServiceCollection AddDbLocalizationProvider(
            this IServiceCollection services,
            Action<ConfigurationContext> setup = null)
        {
            var ctx = ConfigurationContext.Current;
            var factory = ctx.TypeFactory;

            // setup default implementations
            factory.ForQuery<GetAllResources.Query>().DecorateWith<CachedGetAllResourcesHandler>();
            factory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            factory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

            var provider = services.BuildServiceProvider();

            // set to default in-memory provider
            var cache = provider.GetService<IMemoryCache>();
            if (cache != null)
            {
                ctx.CacheManager = new InMemoryCacheManager(cache);
                services.AddSingleton(ctx.CacheManager);
            }

            // run custom configuration setup (if any)
            setup?.Invoke(ctx);

            // adding mvc localization stuff

            var scanState = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(scanState);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var expressionHelper = new ExpressionHelper(keyBuilder);
            var localizationProvider = new LocalizationProvider(keyBuilder, expressionHelper);

            services.AddSingleton(scanState);
            services.AddSingleton(keyBuilder);
            services.AddSingleton(expressionHelper);

            services.AddSingleton(new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, scanState),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, scanState),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, scanState)
            }));

            services.AddSingleton(localizationProvider);
            services.AddSingleton<ILocalizationProvider>(localizationProvider);
            services.AddTransient<ISynchronizer, Synchronizer>();
            services.AddTransient<Synchronizer>();

            services.AddSingleton<DbStringLocalizerFactory>();
            services.AddSingleton<IStringLocalizerFactory>(p => p.GetRequiredService<DbStringLocalizerFactory>());
            services.AddSingleton<DbHtmlLocalizerFactory>();
            services.AddSingleton<IHtmlLocalizerFactory>(p => p.GetRequiredService<DbHtmlLocalizerFactory>());
            services.AddTransient<IViewLocalizer, DbViewLocalizer>();
            services.AddTransient(typeof(IHtmlLocalizer<>), typeof(DbHtmlLocalizer<>));

            // we need to check whether invariant fallback is correctly configured
            if (ctx.EnableInvariantCultureFallback && !ctx.FallbackCultures.Contains(CultureInfo.InvariantCulture))
            {
                ctx.FallbackCultures.Then(CultureInfo.InvariantCulture);
            }

            // setup model metadata providers
            if (ctx.ModelMetadataProviders.ReplaceProviders)
            {
                services.Configure<MvcOptions>(
                    _ =>
                    {
                        _.ModelMetadataDetailsProviders.Add(
                            new LocalizedDisplayMetadataProvider(
                                new ModelMetadataLocalizationHelper(localizationProvider, keyBuilder)));
                    });

                services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, ConfigureMvcViews>());
            }

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
