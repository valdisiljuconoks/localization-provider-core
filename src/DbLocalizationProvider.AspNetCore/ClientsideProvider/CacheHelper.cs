// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.AspNetCore.Cache;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider;

internal class CacheHelper
{
    private static readonly string _separator = "_|_";

    public static string GenerateKey(string filename, string language, bool isDebugMode, bool camelCase)
    {
        return $"{filename}{_separator}{language}__{(isDebugMode ? "debug" : "release")}__{camelCase}";
    }

    public static string GetContainerName(string key)
    {
        if(key == null) throw new ArgumentNullException(nameof(key));

        return !key.Contains(_separator) ? null : key.Substring(0, key.IndexOf(_separator, StringComparison.Ordinal));
    }

    public static void CacheManagerOnRemove(CacheEventArgs args, ICacheManager cache)
    {
        // TODO: implement IEnumerable on cache manager
        using(var existingKeys = InMemoryCacheManager.Entries.GetEnumerator())
        {
            var entriesToRemove = new List<string>();
            while(existingKeys.MoveNext())
            {
                var key = CacheKeyHelper.GetResourceKeyFromCacheKey(existingKeys.Current.Key);
                var containerName = GetContainerName(key);

                if(containerName != null && args.ResourceKey.StartsWith(containerName, StringComparison.InvariantCultureIgnoreCase))
                {
                    entriesToRemove.Add(existingKeys.Current.Key);
                }
            }

            foreach(var entry in entriesToRemove) cache.Remove(entry);
        }
    }
}
