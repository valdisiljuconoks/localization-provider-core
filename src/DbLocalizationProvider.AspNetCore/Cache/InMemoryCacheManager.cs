// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Concurrent;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace DbLocalizationProvider.AspNetCore.Cache;

public class InMemoryCacheManager : ICacheManager
{
    // this is used in cache helper to enumerate over known entries and remove what's needed
    // implemented because there is no way to enumerate keys using built-in cache provider
    internal static readonly ConcurrentDictionary<string, bool> Entries = new ConcurrentDictionary<string, bool>();
    private readonly IMemoryCache _memCache;

    public InMemoryCacheManager(IMemoryCache memCache)
    {
        _memCache = memCache;
    }

    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        _memCache.Set(key, value);
        Entries.TryAdd(key, true);
    }

    public object Get(string key)
    {
        return _memCache.Get(key);
    }

    public void Remove(string key)
    {
        _memCache.Remove(key);
        Entries.TryRemove(key, out _);
    }

    public event CacheEventHandler OnInsert;
    public event CacheEventHandler OnRemove;
}
