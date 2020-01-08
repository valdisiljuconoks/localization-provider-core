﻿// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.Commands
{
    public class CreateOrUpdateTranslationHandler : ICommandHandler<CreateOrUpdateTranslation.Command>
    {
        public void Execute(CreateOrUpdateTranslation.Command command)
        {
            using(var db = new LanguageEntities())
            {
                var resource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == command.Key);

                if(resource == null)
                {
                    // TODO: return some status response obj
                    return;
                }

                var translation = resource.Translations.FirstOrDefault(t => t.Language == command.Language.Name);

                if(translation != null)
                {
                    // update existing translation
                    translation.Value = command.Translation;
                }
                else
                {
                    var newTranslation = new LocalizationResourceTranslation
                                         {
                                             Value = command.Translation,
                                             Language = command.Language.Name,
                                             ResourceId = resource.Id
                                         };

                    db.LocalizationResourceTranslations.Add(newTranslation);
                }

                resource.ModificationDate = DateTime.UtcNow;
                resource.IsModified = true;
                db.SaveChanges();
            }

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
