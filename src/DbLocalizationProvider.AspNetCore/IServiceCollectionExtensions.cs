// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.AspNetCore.Commands;
using DbLocalizationProvider.AspNetCore.DataAnnotations;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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

            factory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
            factory.ForQuery<GetAllResources.Query>().DecorateWith<CachedGetAllResourcesHandler>();
            factory.ForQuery<GetResource.Query>().SetHandler<GetResourceHandler>();
            factory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();
            factory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            factory.ForCommand<ClearCache.Command>().SetHandler<ClearCacheHandler>();

            factory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();
            factory.ForCommand<RemoveTranslation.Command>().SetHandler<RemoveTranslationHandler>();

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

            // get connection string from configuration providers
            var configProvider = provider.GetService<IConfiguration>();
            ConfigurationContext.Current.DbContextConnectionString = configProvider.GetConnectionString(ConfigurationContext.Current.Connection);

            services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
            services.AddSingleton(_ => LocalizationProvider.Current);

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
