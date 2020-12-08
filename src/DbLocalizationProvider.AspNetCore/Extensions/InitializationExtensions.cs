// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AspNetCore.ServiceLocators;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DbLocalizationProvider.AspNetCore.Extensions
{
    /// <summary>
    /// Extension points
    /// </summary>
    public static class InitializationExtensions
    {
        private static bool SyncOnStartDone = false;
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
            // TODO: Hack! This code should be removed after making library DI compatible.
            var serviceProvider =  builder.ApplicationServices.GetRequiredService<IServiceProvider>();
            ServiceLocator.Initialize(serviceProvider.GetService<IServiceProviderProxy>());
            // --

            // TODO Andriy: bad idea to sync here cuz HttpContext does not exists, so Service locator will not work. It could be changed when TypeFactory get redesigned for DI support
            // if we need to sync - then it's good time to do it now

            builder.UseWhen(context => !SyncOnStartDone,
                            builder =>
                            {
                                builder.Use(async (context, next) =>
                                {
                                    var logger = builder?.ApplicationServices.GetService<ILogger<LoggerAdapter>>();

                                    var configContext = ConfigurationContext.Current;

                                    if (logger != null) configContext.Logger = new LoggerAdapter(logger);

                                    var sync = new Synchronizer();
                                    sync.SyncResources(configContext.DiscoverAndRegisterResources);

                                    if (!configContext.DiscoverAndRegisterResources)
                                    {
                                        configContext.Logger?.Info(
                                            $"{nameof(configContext.DiscoverAndRegisterResources)}=false. Resource synchronization skipped.");
                                    }

                                    SyncOnStartDone = true;

                                    await next.Invoke();
                                });
                            });

            return builder;
        }
    }
}
