// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Extension to initialize and setup provider.
/// </summary>
public static class IServiceProviderExtensions
{
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

        var context = serviceFactory.GetRequiredService<ConfigurationContext>();
        var usageConfigurator = serviceFactory.GetService<IUsageConfigurator>();

        if (usageConfigurator != null)
        {
            usageConfigurator.Configure(context, serviceFactory);
        }

        // if we need to sync - then it's good time to do it now
        var sync = serviceFactory.GetRequiredService<Synchronizer>();
        sync.SyncResources(context.DiscoverAndRegisterResources);

        if (!context.DiscoverAndRegisterResources)
        {
            context.Logger?.Info($"{nameof(context.DiscoverAndRegisterResources)}=false. Resource synchronization skipped.");
        }

        foreach (var provider in serviceFactory.GetServices<IManualResourceProvider>())
        {
            sync.RegisterManually(provider.GetResources());
        }

        context.Logger?.Info("DbLocalizationProvider initialized.");
    }
}
