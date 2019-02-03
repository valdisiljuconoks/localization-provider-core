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
                ConfigurationContext.Current.CacheManager = new InMemoryCacheManager(cache);

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

            return services;
        }
    }
}
