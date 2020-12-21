// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Extension points
    /// </summary>
    public static class InitializationExtensions
    {
        /// <summary>
        /// Synchronizes resources with underlying storage
        /// </summary>
        public static void UseDbLocalizationProvider()
        {
            UseDbLocalizationProvider(null);
        }

        /// <summary>
        /// Synchronizes resources with underlying storage
        /// </summary>
        /// <param name="builder">ASP.NET Core application builder</param>
        /// <returns>ASP.NET Core application builder to enable fluent API call chains</returns>
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

            if (context.ManualResourceProvider != null)
            {
                sync.RegisterManually(context.ManualResourceProvider.GetResources());
            }

            return builder;
        }
    }
}
