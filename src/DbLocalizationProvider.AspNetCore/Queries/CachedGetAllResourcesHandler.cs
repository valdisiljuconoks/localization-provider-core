using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    public class CachedGetAllResourcesHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        private readonly IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>> _inner;

        public CachedGetAllResourcesHandler(IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>> inner)
        {
            _inner = inner;
        }

        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            if(query.ForceReadFromDb)
                return _inner.Execute(query);

            // get keys for known resources
            var keys = ConfigurationContext.Current.BaseCacheManager.KnownResourceKeys.Keys;

            // if keys = 0, execute inner query to actually get resources from the db
            // this is usually called during initialization when cache is not yet filled up
            if(keys.Count == 0)
                return _inner.Execute(query);

            var result = new List<LocalizationResource>();

            foreach(var key in keys)
            {
                var cacheKey = CacheKeyHelper.BuildKey(key);
                if(ConfigurationContext.Current.CacheManager.Get(cacheKey) is LocalizationResource localizationResource)
                {
                    result.Add(localizationResource);
                }
                else
                {
                    // failed to get from cache, should call database
                    var resourceFromDb = new GetResource.Query(key).Execute();
                    if(resourceFromDb != null)
                    {
                        ConfigurationContext.Current.CacheManager.Insert(cacheKey, resourceFromDb);
                        result.Add(resourceFromDb);
                    }
                }
            }

            return result;
        }
    }
}
