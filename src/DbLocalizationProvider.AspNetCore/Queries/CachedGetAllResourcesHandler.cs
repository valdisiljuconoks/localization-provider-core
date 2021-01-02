// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    /// <summary>
    /// Cached version of `GetAllResources` query
    /// </summary>
    public class CachedGetAllResourcesHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        private readonly IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>> _inner;
        private readonly ConfigurationContext _configurationContext;

        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        /// <param name="inner">Inner query</param>
        /// <param name="configurationContext">Configuration settings</param>
        public CachedGetAllResourcesHandler(IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>> inner, ConfigurationContext configurationContext)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _configurationContext = configurationContext ?? throw new ArgumentNullException(nameof(configurationContext));
        }

        /// <summary>
        /// Executes the handler
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>Result from the query.</returns>
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            if(query.ForceReadFromDb) return _inner.Execute(query);

            // if keys = 0, execute inner query to actually get resources from the db
            // this is usually called during initialization when cache is not yet filled up
            if(_configurationContext.BaseCacheManager.KnownKeyCount == 0) return _inner.Execute(query);

            var result = new List<LocalizationResource>();
            var keys = _configurationContext.BaseCacheManager.KnownKeys;

            foreach(var key in keys)
            {
                var cacheKey = CacheKeyHelper.BuildKey(key);
                if(_configurationContext.CacheManager.Get(cacheKey) is LocalizationResource localizationResource)
                {
                    result.Add(localizationResource);
                }
                else
                {
                    // failed to get from cache, should call database
                    var resourceFromDb = new GetResource.Query(key).Execute();
                    if(resourceFromDb != null)
                    {
                        _configurationContext.CacheManager.Insert(cacheKey, resourceFromDb, true);
                        result.Add(resourceFromDb);
                    }
                }
            }

            return result;
        }
    }
}
