// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder)
        {
            // create db schema
            using(var ctx = new LanguageEntities())
                ctx.Database.Migrate();

            var synchronizer = new ResourceSynchronizer();
            synchronizer.DiscoverAndRegister();

            // in cases when there has been already a call to LocalizationProvider.Current (some static weird things)
            // and only then setup configuration is ran - here we need to reset instance once again with new settings
            LocalizationProvider.Initialize();

            return builder;
        }
    }
}
