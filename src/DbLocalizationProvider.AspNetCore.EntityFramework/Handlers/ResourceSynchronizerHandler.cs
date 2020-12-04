// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.EntityFramework.Entities;
using DbLocalizationProvider.AspNetCore.ServiceLocators;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.EntityFramework.Handlers
{
    public class ResourceSynchronizerHandler : IQueryHandler<SyncResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(SyncResources.Query query)
        {
            ConfigurationContext.Current.Logger?.Debug("Starting to sync resources...");
            var sw = new Stopwatch();
            sw.Start();

            var discoveredResources = query.DiscoveredResources;
            var discoveredModels = query.DiscoveredModels;

            ResetSyncStatus();

            var allResources = new GetAllResources.Query(true).Execute();
            Parallel.Invoke(() => RegisterDiscoveredResources(discoveredResources, allResources),
                            () => RegisterDiscoveredResources(discoveredModels, allResources));

            var result = MergeLists(allResources, discoveredResources.ToList(), discoveredModels.ToList());
            sw.Stop();

            ConfigurationContext.Current.Logger?.Debug($"Resource synchronization took: {sw.ElapsedMilliseconds}ms");

            return result;
        }

        private DbContext GetDbContextInstance()
        {
            var result = ServiceLocator.ServiceProvider.GetService(Settings.ContextType) as DbContext;
            return result;
        }

        internal IEnumerable<LocalizationResource> MergeLists(
            IEnumerable<LocalizationResource> databaseResources,
            List<DiscoveredResource> discoveredResources,
            List<DiscoveredResource> discoveredModels)
        {
            if (discoveredResources == null || discoveredModels == null || !discoveredResources.Any() ||
                !discoveredModels.Any())
                return databaseResources;

            var result = new List<LocalizationResource>(databaseResources);
            var dic = result.ToDictionary(r => r.ResourceKey, r => r);

            // run through resources
            CompareAndMerge(ref discoveredResources, dic, ref result);
            CompareAndMerge(ref discoveredModels, dic, ref result);

            return result;
        }

        private static void CompareAndMerge(
            ref List<DiscoveredResource> discoveredResources,
            Dictionary<string, LocalizationResource> dic,
            ref List<LocalizationResource> result)
        {
            while (discoveredResources.Count > 0)
            {
                var discoveredResource = discoveredResources[0];
                if (!dic.ContainsKey(discoveredResource.Key))
                {
                    // there is no resource by this key in db - we can safely insert
                    result.Add(new LocalizationResource(discoveredResource.Key)
                    {
                        Translations = discoveredResource.Translations.Select(t =>
                                                                                  new LocalizationResourceTranslation
                                                                                  {
                                                                                      Language = t.Culture,
                                                                                      Value = t.Translation
                                                                                  }).ToList()
                    });
                }
                else
                {
                    // resource exists in db - we need to merge only unmodified translations
                    var existingRes = dic[discoveredResource.Key];
                    if (!existingRes.IsModified.HasValue || !existingRes.IsModified.Value)
                    {
                        // resource is unmodified in db - overwrite
                        foreach (var translation in discoveredResource.Translations)
                        {
                            var t = existingRes.Translations.FindByLanguage(translation.Culture);
                            if (t == null)
                            {
                                existingRes.Translations.Add(new LocalizationResourceTranslation
                                {
                                    Language = translation.Culture,
                                    Value = translation.Translation
                                });
                            }
                            else
                            {
                                t.Language = translation.Culture;
                                t.Value = translation.Translation;
                            }
                        }
                    }
                    else
                    {
                        // resource exists in db, is modified - we need to update only invariant translation
                        var t = existingRes.Translations.FindByLanguage(CultureInfo.InvariantCulture);
                        var invariant =
                            discoveredResource.Translations.FirstOrDefault(t2 => t.Language == string.Empty);
                        if (t != null && invariant != null)
                        {
                            t.Language = invariant.Culture;
                            t.Value = invariant.Translation;
                        }
                    }
                }

                discoveredResources.Remove(discoveredResource);
            }
        }

        private void ResetSyncStatus()
        {
            var context = GetDbContextInstance();

            context.Set<LocalizationResource>()
                .Where(p => p.FromCode)
                .AsEnumerable().ForEach(p => p.FromCode = false);

            context.SaveChanges();
        }

        private void RegisterDiscoveredResources(
            ICollection<DiscoveredResource> properties,
            IEnumerable<LocalizationResource> allResources)
        {
            // split work queue by 400 resources each
            var groupedProperties = properties.SplitByCount(400);

            if (!groupedProperties.Any()) return;


            var context = GetDbContextInstance();

            Parallel.ForEach(groupedProperties,
                             group =>
                             {
                                 var refactoredResources = group.Where(r => !string.IsNullOrEmpty(r.OldResourceKey));

                                 foreach (var refactoredResource in refactoredResources)
                                 {
                                     context.Set<LocalizationResourceEntity>()
                                         .Where(p => p.ResourceKey == refactoredResource.OldResourceKey)
                                         .AsEnumerable()
                                         .ForEach(p => p.FromCode = true);
                                 }

                                 foreach (var property in group)
                                 {
                                     var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == property.Key);

                                     if (existingResource == null)
                                     {
                                         var entity = context.Set<LocalizationResourceEntity>()
                                             .SingleOrDefault(p => p.ResourceKey == property.Key);

                                         if (entity == null)
                                         {
                                             entity = new LocalizationResourceEntity
                                             {
                                                 ResourceKey = property.Key,
                                                 ModificationDate = DateTime.UtcNow,
                                                 Author = "type-scanner",
                                                 FromCode = true,
                                                 IsModified = false,
                                                 IsHidden = property.IsHidden,
                                                 Translations = new List<LocalizationResourceTranslationEntity>()
                                             };
                                             context.Add(entity);
                                         }

                                         // add all translations
                                         foreach (var propertyTranslation in property.Translations)
                                         {
                                             entity.Translations.Add(new LocalizationResourceTranslationEntity
                                             {
                                                 Language = propertyTranslation.Culture,
                                                 Value = propertyTranslation.Translation.Replace(
                                                     "'",
                                                     "''"),
                                                 ModificationDate = DateTime.UtcNow
                                             });
                                         }
                                     }

                                     if (existingResource != null)
                                     {
                                         context.Set<LocalizationResourceEntity>()
                                             .Where(p => p.Id == existingResource.Id)
                                             .AsEnumerable()
                                             .ForEach(p =>
                                             {
                                                 p.FromCode = true;
                                                 p.IsHidden = property.IsHidden;
                                             });

                                         var invariantTranslation = property.Translations.First(t => t.Culture == string.Empty);

                                         context.Set<LocalizationResourceTranslationEntity>()
                                             .Where(p => p.ResourceId == existingResource.Id &&
                                                         p.Language == invariantTranslation.Culture)
                                             .AsEnumerable()
                                             .ForEach(p => p.Value = invariantTranslation.Translation.Replace("'", "''"));

                                         if (existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                                             foreach (var propertyTranslation in property.Translations)
                                             {
                                                 var existingTranslation =
                                                     existingResource.Translations.FirstOrDefault(
                                                         t => t.Language == propertyTranslation.Culture);
                                                 if (existingTranslation == null)
                                                 {
                                                     var entity = new LocalizationResourceTranslationEntity
                                                     {
                                                         ResourceId = existingResource.Id,
                                                         Language = propertyTranslation.Culture,
                                                         Value = propertyTranslation.Translation.Replace("'", "''"),
                                                         ModificationDate = DateTime.UtcNow
                                                     };
                                                     context.Add(entity);
                                                 }
                                                 else if (!existingTranslation.Value.Equals(propertyTranslation.Translation))
                                                 {
                                                     context.Set<LocalizationResourceTranslationEntity>()
                                                         .Where(p => p.ResourceId == existingResource.Id &&
                                                                     p.Language == propertyTranslation.Culture)
                                                         .AsEnumerable()
                                                         .ForEach(p => p.Value =
                                                                      propertyTranslation.Translation.Replace("'", "''"));
                                                 }
                                             }
                                     }

                                     context.SaveChanges();
                                 }
                             });
        }
    }
}
