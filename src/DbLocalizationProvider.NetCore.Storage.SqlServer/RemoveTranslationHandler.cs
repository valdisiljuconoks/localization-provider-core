// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer
{
    public class RemoveTranslationHandler : ICommandHandler<RemoveTranslation.Command>
    {
        public void Execute(RemoveTranslation.Command command)
        {
            using(var db = new LanguageEntities())
            {
                var resource = db.LocalizationResources.Include(r => r.Translations)
                    .FirstOrDefault(r => r.ResourceKey == command.Key);

                if(resource == null) return;

                if(!resource.IsModified.HasValue || !resource.IsModified.Value)
                    throw new InvalidOperationException($"Cannot delete translation for unmodified resource `{command.Key}`");

                var t = resource.Translations.FirstOrDefault(_ => _.Language == command.Language.Name);
                if(t != null)
                {
                    db.LocalizationResourceTranslations.Remove(t);
                    db.SaveChanges();
                }
            }

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
