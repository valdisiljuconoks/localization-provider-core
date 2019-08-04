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
                        ConfigurationContext.Current.CacheManager.Insert(cacheKey, resourceFromDb, true);
                        result.Add(resourceFromDb);
                    }
                }
            }

            return result;
        }
    }
}
