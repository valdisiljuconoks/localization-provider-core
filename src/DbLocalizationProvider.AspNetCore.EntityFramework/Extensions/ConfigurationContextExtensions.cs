// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.EntityFramework.Handlers;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.EntityFramework.Extensions
{
    /// <summary>
    /// Extension method to provide nice way to configure EntityFramework as resource storage.
    /// </summary>
    public static class ConfigurationContextExtensions
    {
        /// <summary>
        /// If you can afford EntityFramework - this method is for you.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static ConfigurationContext UseEntityFramework<T>(this ConfigurationContext context)
            where T : DbContext
        {
            StorageSettings.ContextType = typeof(T);

            ConfigurationContext.Current.TypeFactory.ForQuery<UpdateSchema.Command>()
                .SetHandler<SchemaUpdaterHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<SyncResources.Query>()
                .SetHandler<ResourceSynchronizerHandler>();

            ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>()
                .SetHandler<AvailableLanguagesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>()
                .SetHandler<GetAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetResource.Query>().SetHandler<GetResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>()
                .SetHandler<GetTranslationHandler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResources.Command>()
                .SetHandler<CreateNewResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteAllResources.Command>()
                .SetHandler<DeleteAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>()
                .SetHandler<DeleteResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<RemoveTranslation.Command>()
                .SetHandler<RemoveTranslationHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>()
                .SetHandler<CreateOrUpdateTranslationHandler>();

            return context;
        }
    }
}
