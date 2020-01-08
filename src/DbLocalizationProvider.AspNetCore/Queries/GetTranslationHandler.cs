// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    public class GetTranslationHandler : GetTranslation.GetTranslationHandlerBase, IQueryHandler<GetTranslation.Query, string>
    {
        public string Execute(GetTranslation.Query query)
        {
            if(!ConfigurationContext.Current.EnableLocalization())
                return query.Key;

            var key = query.Key;
            var cacheKey = CacheKeyHelper.BuildKey(key);
            var localizationResource = ConfigurationContext.Current.CacheManager.Get(cacheKey) as LocalizationResource;

            if(localizationResource == null)
            {
                // resource is not found in the cache, let's check database
                localizationResource = GetResourceFromDb(key) ?? LocalizationResource.CreateNonExisting(key);
                ConfigurationContext.Current.CacheManager.Insert(cacheKey, localizationResource, true);
            }

            var fallbackCultures = ConfigurationContext.Current.FallbackCultures;
            return fallbackCultures != null && fallbackCultures.Any()
                       ? base.GetTranslationWithFallback(localizationResource.Translations, query.Language, fallbackCultures, query.UseFallback)?.Value
                       : base.GetTranslationFromAvailableList(localizationResource.Translations, query.Language, query.UseFallback)?.Value;
        }

        protected virtual LocalizationResource GetResourceFromDb(string key)
        {
            using(var db = new LanguageEntities())
            {
                var resource = db.LocalizationResources
                    .Include(r => r.Translations)
                    .FirstOrDefault(r => r.ResourceKey == key);

                return resource;
            }
        }
    }
}
