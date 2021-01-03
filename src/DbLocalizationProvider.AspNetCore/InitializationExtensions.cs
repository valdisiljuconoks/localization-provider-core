// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Extension point to initialize provider.
    /// </summary>
    public static class InitializationExtensions
    {
        /// <summary>
        /// Synchronizes resources with underlying storage
        /// </summary>
        /// <param name="builder">ASP.NET Core application builder</param>
        /// <returns>ASP.NET Core application builder to enable fluent API call chains</returns>
        public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            UseDbLocalizationProvider(builder.ApplicationServices);

            return builder;
        }

        /// <summary>
        /// Synchronizes resources with underlying storage
        /// </summary>
        /// <param name="serviceFactory">Factory of the services (this will be required to get access to previously registered services)</param>
        /// <returns>ASP.NET Core application builder to enable fluent API call chains</returns>
        public static void UseDbLocalizationProvider(this IServiceProvider serviceFactory)
        {
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            var logger = serviceFactory.GetRequiredService<ILogger>();
            var context = serviceFactory.GetService<ConfigurationContext>();

            context.Logger = logger;

            // if we need to sync - then it's good time to do it now
            var sync = serviceFactory.GetService<Synchronizer>();
            sync.SyncResources(context.DiscoverAndRegisterResources);

            if (!context.DiscoverAndRegisterResources)
            {
                logger.Info($"{nameof(context.DiscoverAndRegisterResources)}=false. Resource synchronization skipped.");
            }

            if (context.ManualResourceProvider != null)
            {
                sync.RegisterManually(context.ManualResourceProvider.GetResources());
            }
        }
    }
}
