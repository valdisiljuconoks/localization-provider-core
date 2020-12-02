// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.Storage.Repositories;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.AspNetCore.Storage.Handlers
{
    /// <summary>
    ///     Implementation for creating or updating existing translation
    /// </summary>
    public class CreateOrUpdateTranslationHandler : ICommandHandler<CreateOrUpdateTranslation.Command>
    {
        /// <summary>
        ///     Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(CreateOrUpdateTranslation.Command command)
        {
            var repository = new ResourceRepository();
            var resource = repository.GetByKey(command.Key);
            var now = DateTime.UtcNow;

            if (resource == null) return;

            var translation = resource.Translations.FindByLanguage(command.Language);

            if (translation == null)
            {
                var newTranslation = new LocalizationResourceTranslation
                {
                    Value = command.Translation,
                    Language = command.Language.Name,
                    ResourceId = resource.Id,
                    ModificationDate = now
                };

                repository.AddTranslation(resource, newTranslation);
            }
            else
            {
                translation.Value = command.Translation;
                translation.ModificationDate = now;
                repository.UpdateTranslation(resource, translation);
            }

            resource.ModificationDate = now;
            resource.IsModified = true;

            repository.UpdateResource(resource);

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}