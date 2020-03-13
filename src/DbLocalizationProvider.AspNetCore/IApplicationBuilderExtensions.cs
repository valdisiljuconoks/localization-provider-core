// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder)
        {
            var logger = builder?.ApplicationServices.GetService<ILogger<LoggerAdapter>>();
            var context = ConfigurationContext.Current;

            if (logger != null) context.Logger = new LoggerAdapter(logger);

            // if we need to sync - then it's good time to do it now
            var sync = new Synchronizer();
            sync.SyncResources(context.DiscoverAndRegisterResources);

            if (!context.DiscoverAndRegisterResources)
            {
                context.Logger?.Info($"{nameof(context.DiscoverAndRegisterResources)}=false. Resource synchronization skipped.");
            }

            return builder;
        }
    }
}
