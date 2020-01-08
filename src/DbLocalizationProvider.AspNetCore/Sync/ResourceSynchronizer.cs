﻿// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.Sync
{
    public class ResourceSynchronizer
    {
        public void DiscoverAndRegister()
        {
            if(!ConfigurationContext.Current.DiscoverAndRegisterResources)
                return;

            var discoveredTypes = TypeDiscoveryHelper.GetTypes(t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                                                               t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            var discoveredResources = discoveredTypes[0];
            var discoveredModels = discoveredTypes[1];
            var foreignResources = ConfigurationContext.Current.ForeignResources;
            if(foreignResources != null && foreignResources.Any())
            {
                discoveredResources.AddRange(foreignResources.Select(x => x.ResourceType));
            }

            ResetSyncStatus();
            var allResources = new GetAllResources.Query().Execute();

            Parallel.Invoke(() => RegisterDiscoveredResources(discoveredResources, allResources),
                            () => RegisterDiscoveredResources(discoveredModels, allResources));

            StoreKnownResourcesAndPopulateCache();
        }

        public void RegisterManually(IEnumerable<ManualResource> resources)
        {
            using(var db = new LanguageEntities())
            {
                var defaultCulture = new DetermineDefaultCulture.Query().Execute();

                foreach(var resource in resources)
                    RegisterIfNotExist(db, resource.Key, resource.Translation, defaultCulture, "manual");

                db.SaveChanges();
            }
        }

        private void StoreKnownResourcesAndPopulateCache()
        {
            var allResources = new GetAllResources.Query(true).Execute();

            if(ConfigurationContext.Current.PopulateCacheOnStartup)
            {
                new ClearCache.Command().Execute();
                foreach(var resource in allResources)
                {
                    var key = CacheKeyHelper.BuildKey(resource.ResourceKey);
                    ConfigurationContext.Current.CacheManager.Insert(key, resource, true);
                }
            }
            else
            {
                // just store known resource keys in cache
                allResources.ForEach(r => ConfigurationContext.Current.BaseCacheManager.StoreKnownKey(r.ResourceKey));
            }
        }

        private void ResetSyncStatus()
        {
            using(var conn = new SqlConnection(ConfigurationContext.Current.DbContextConnectionString))
            {
                var cmd = new SqlCommand("UPDATE dbo.LocalizationResources SET FromCode = 0", conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void RegisterDiscoveredResources(IEnumerable<Type> types, IEnumerable<LocalizationResource> allResources)
        {
            var helper = new TypeDiscoveryHelper();
            var properties = types.SelectMany(type => helper.ScanResources(type)).DistinctBy(r => r.Key);

            // split work queue by 400 resources each
            var groupedProperties = properties.SplitByCount(400);

            Parallel.ForEach(groupedProperties,
                             group =>
                             {
                                 var sb = new StringBuilder();
                                 sb.AppendLine("declare @resourceId int");

                                 var refactoredResources = group.Where(r => !string.IsNullOrEmpty(r.OldResourceKey));
                                 foreach(var refactoredResource in refactoredResources)
                                 {
                                     sb.Append($@"
        if exists(select 1 from LocalizationResources with(nolock) where ResourceKey = '{refactoredResource.OldResourceKey}')
        begin
            update dbo.LocalizationResources set ResourceKey = '{refactoredResource.Key}', FromCode = 1 where ResourceKey = '{refactoredResource.OldResourceKey}'
        end
        ");
                                 }

                                 foreach(var property in group)
                                 {
                                     var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == property.Key);

                                     if(existingResource == null)
                                     {
                                         sb.Append($@"
        set @resourceId = isnull((select id from LocalizationResources where [ResourceKey] = '{property.Key}'), -1)
        if (@resourceId = -1)
        begin
            insert into LocalizationResources ([ResourceKey], ModificationDate, Author, FromCode, IsModified, IsHidden)
            values ('{property.Key}', getutcdate(), 'type-scanner', 1, 0, {Convert.ToInt32(property.IsHidden)})
            set @resourceId = SCOPE_IDENTITY()");

                                                 // add all translations
                                                 foreach(var propertyTranslation in property.Translations)
                                         {
                                             sb.Append($@"
            insert into LocalizationResourceTranslations (ResourceId, [Language], [Value]) values (@resourceId, '{propertyTranslation.Culture}', N'{
                                                               propertyTranslation.Translation.Replace("'", "''")
                                                           }')
        ");
                                         }

                                         sb.Append(@"
        end
        ");
                                     }

                                     if(existingResource != null)
                                     {
                                         sb.AppendLine($"update LocalizationResources set FromCode = 1, IsHidden = {Convert.ToInt32(property.IsHidden)} where [Id] = {existingResource.Id}");

                                         var invariantTranslation = property.Translations.First(t => t.Culture == string.Empty);
                                         sb.AppendLine($"update LocalizationResourceTranslations set [Value] = N'{invariantTranslation.Translation.Replace("'", "''")}' where ResourceId={existingResource.Id} and [Language]='{invariantTranslation.Culture}'");

                                         if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                                         {
                                             foreach(var propertyTranslation in property.Translations)
                                                 AddTranslationScript(existingResource, sb, propertyTranslation);
                                         }
                                     }
                                 }

                                 using(var conn = new SqlConnection(ConfigurationContext.Current.DbContextConnectionString))
                                 {
                                     var cmd = new SqlCommand(sb.ToString(), conn)
                                     {
                                         CommandTimeout = 60
                                     };

                                     conn.Open();
                                     cmd.ExecuteNonQuery();
                                     conn.Close();
                                 }
                             });
        }

        private static void AddTranslationScript(LocalizationResource existingResource, StringBuilder buffer, DiscoveredTranslation resource)
        {
            var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == resource.Culture);
            if(existingTranslation == null)
            {
                buffer.Append($@"
        insert into LocalizationResourceTranslations (ResourceId, [Language], [Value]) values ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}')
        ");
            }
            else if(!existingTranslation.Value.Equals(resource.Translation))
            {
                buffer.Append($@"
        update LocalizationResourceTranslations set [Value] = N'{resource.Translation.Replace("'", "''")}' where ResourceId={existingResource.Id} and [Language]='{resource.Culture}'
        ");
            }
        }

        private void RegisterIfNotExist(LanguageEntities db, string resourceKey, string resourceValue, string defaultCulture, string author = "type-scanner")
        {
            var existingResource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == resourceKey);

            if(existingResource != null)
            {
                existingResource.FromCode = true;

                // if resource is not modified - we can sync default value from code
                if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                {
                    existingResource.ModificationDate = DateTime.UtcNow;
                    var defaultTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == defaultCulture);
                    if(defaultTranslation != null)
                    {
                        defaultTranslation.Value = resourceValue;
                    }
                }

                var fromCodeTranslation = existingResource.Translations.FindByLanguage(CultureInfo.InvariantCulture);
                if(fromCodeTranslation != null)
                {
                    fromCodeTranslation.Value = resourceValue;
                }
                else
                {
                    fromCodeTranslation = new LocalizationResourceTranslation
                    {
                        Language = CultureInfo.InvariantCulture.Name,
                        Value = resourceValue
                    };

                    existingResource.Translations.Add(fromCodeTranslation);
                }
            }
            else
            {
                // create new resource
                var resource = new LocalizationResource(resourceKey)
                {
                    ModificationDate = DateTime.UtcNow,
                    Author = author,
                    FromCode = true,
                    IsModified = false
                };

                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = defaultCulture,
                    Value = resourceValue
                });

                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = CultureInfo.InvariantCulture.Name,
                    Value = resourceValue
                });

                db.LocalizationResources.Add(resource);
            }
        }
    }
}
