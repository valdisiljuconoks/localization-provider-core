// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder)
        {
            // if we need to sync - then it's good time to do it now
            if(ConfigurationContext.Current.DiscoverAndRegisterResources)
            {
                var sync = new Synchronizer();
                sync.SyncResources();
            }
            else
            {
                // TODO: add logger adapter and write that sync has been skipped
            }

            // in cases when there has been already a call to LocalizationProvider.Current (some static weird things)
            // and only then setup configuration is ran - here we need to reset instance once again with new settings
            LocalizationProvider.Initialize();

            return builder;
        }
    }
}
