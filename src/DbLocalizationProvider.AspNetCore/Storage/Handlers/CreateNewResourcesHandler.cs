// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.Storage.Repositories;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.AspNetCore.Storage.Handlers
{
    /// <summary>
    ///     Implementation of the command to create new resources
    /// </summary>
    public class CreateNewResourcesHandler : ICommandHandler<CreateNewResources.Command>
    {
        /// <summary>
        ///     Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        /// <exception cref="InvalidOperationException">Resource with key `{resource.ResourceKey}` already exists</exception>
        public void Execute(CreateNewResources.Command command)
        {
            if (command.LocalizationResources == null || !command.LocalizationResources.Any()) return;


            foreach (var resource in command.LocalizationResources)
            {
                var repository = new ResourceRepository();
                var existingResource = repository.GetByKey(resource.ResourceKey);

                if (existingResource != null)
                    throw new InvalidOperationException($"Resource with key `{resource.ResourceKey}` already exists");

                resource.ModificationDate = DateTime.UtcNow;

                // if we are importing single translation and it's not invariant
                // set it also as invariant translation
                if (resource.Translations.Count == 1 && resource.Translations.InvariantTranslation() == null)
                {
                    var t = resource.Translations.First();
                    resource.Translations.Add(new LocalizationResourceTranslation
                    {
                        Value = t.Value,
                        Language = string.Empty
                    });
                }

                repository.InsertResource(resource);

                //TODO: ConfigurationContext.Current.BaseCacheManager.StoreKnownKey(resource.ResourceKey);
            }
        }
    }
}