// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.AspNetCore.DataAnnotations;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDbLocalizationProvider(this IServiceCollection services, Action<ConfigurationContext> setup = null)
        {
            // setup default implementations
            var factory = ConfigurationContext.Current.TypeFactory;

            factory.ForQuery<GetAllResources.Query>().DecorateWith<CachedGetAllResourcesHandler>();
            factory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            factory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

            var provider = services.BuildServiceProvider();

            // set to default in-memory provider
            var cache = provider.GetService<IMemoryCache>();
            if(cache != null)
            {
                ConfigurationContext.Current.CacheManager = new InMemoryCacheManager(cache);
                services.AddSingleton(ConfigurationContext.Current.CacheManager);
            }

            // run custom configuration setup (if any)
            setup?.Invoke(ConfigurationContext.Current);

            services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
            services.AddSingleton(_ => LocalizationProvider.Current);
            services.AddSingleton<ILocalizationProvider>(_ => LocalizationProvider.Current);

            // setup model metadata providers
            if(ConfigurationContext.Current.ModelMetadataProviders.ReplaceProviders)
            {
                services.Configure<MvcOptions>(_ =>
                                               {
                                                   _.ModelMetadataDetailsProviders.Add(new LocalizedDisplayMetadataProvider());
                                                   _.ModelValidatorProviders.Add(new LocalizedValidationMetadataProvider());
                                               });

                services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, ConfigureMvcViews>());
            }

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
