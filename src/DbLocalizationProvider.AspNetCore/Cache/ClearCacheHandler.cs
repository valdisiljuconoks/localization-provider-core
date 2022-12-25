// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.AspNetCore.Cache;

public class ClearCacheHandler : ICommandHandler<ClearCache.Command>
{
    private readonly ICacheManager _cache;

    public ClearCacheHandler(ICacheManager cache)
    {
        _cache = cache;
    }

    public void Execute(ClearCache.Command command)
    {
        foreach(var itemToRemove in InMemoryCacheManager.Entries)
        {
            _cache.Remove(itemToRemove.Key);
        }
    }
}
