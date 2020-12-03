// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AspNetCore.EntityFramework.Entities;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.EntityFramework.Repositories
{
    /// <summary>
    /// Repository for working with underlying MSSQL storage
    /// </summary>
    public class ResourceRepository
    {
        private DbContext GetDbContextInstance()
        {
            var result = ServiceLocator.ServiceLocator.ServiceProvider.GetService(Settings.ContextType) as DbContext;
            return result;
        }
        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns>List of resources</returns>
        public IEnumerable<LocalizationResource> GetAll()
        {
            var context = GetDbContextInstance();

            var result = context.Set<LocalizationResourceEntity>()
                .AsNoTracking()
            .Select(r => new LocalizationResource());

            return result;


            //using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            //{
            //    conn.Open();

            //    var cmd = new NpgsqlCommand(@"SELECT
            //            r.""Id"",
            //            r.""ResourceKey"",
            //            r.""Author"",
            //            r.""FromCode"",
            //            r.""IsHidden"",
            //            r.""IsModified"",
            //            r.""ModificationDate"",
            //            r.""Notes"",
            //            t.""Id"" as ""TranslationId"",
            //            t.""Value"" as ""Translation"",
            //            t.""Language"",
            //            t.""ModificationDate"" as ""TranslationModificationDate""
            //            FROM public.""LocalizationResources"" r
            //        LEFT JOIN public.""LocalizationResourceTranslations"" t ON r.""Id"" = t.""ResourceId""",
            //        conn);

            //    var reader = cmd.ExecuteReader();
            //    var lookup = new Dictionary<string, LocalizationResource>();

            //    void CreateTranslation(NpgsqlDataReader sqlDataReader, LocalizationResource localizationResource)
            //    {
            //        if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("TranslationId")))
            //            localizationResource.Translations.Add(new LocalizationResourceTranslation
            //            {
            //                Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TranslationId")),
            //                ResourceId = localizationResource.Id,
            //                Value = sqlDataReader.GetStringSafe("Translation"),
            //                Language = sqlDataReader.GetStringSafe("Language") ?? string.Empty,
            //                ModificationDate = reader.GetDateTime(reader.GetOrdinal("TranslationModificationDate")),
            //                LocalizationResource = localizationResource
            //            });
            //    }

            //    while (reader.Read())
            //    {
            //        var key = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.ResourceKey)));
            //        if (lookup.TryGetValue(key, out var resource))
            //        {
            //            CreateTranslation(reader, resource);
            //        }
            //        else
            //        {
            //            var result = CreateResourceFromSqlReader(key, reader);
            //            CreateTranslation(reader, result);
            //            lookup.Add(key, result);
            //        }
            //    }

            //    return lookup.Values;
            //}
            return new List<LocalizationResource>();
        }

        /// <summary>
        /// Gets resource by the key.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>Localized resource if found by given key</returns>
        /// <exception cref="ArgumentNullException">resourceKey</exception>
        public LocalizationResource GetByKey(string resourceKey)
        {
            if (resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            var context = GetDbContextInstance();

            var result = context.Set<LocalizationResourceEntity>()
                .Include(p => p.Translations)
                .AsNoTracking()
                .Where(p => p.ResourceKey == resourceKey)
                .Select(p => new LocalizationResource()
                {
                    Id = (int)p.Id,
                    Author = p.Author ?? "unknown",
                    FromCode = p.FromCode,
                    ResourceKey = p.ResourceKey,
                    IsHidden = p.IsHidden,
                    IsModified = p.IsModified,
                    ModificationDate = p.ModificationDate,
                    Notes = p.Notes,
                    Translations = p.Translations.Select(t => new LocalizationResourceTranslation
                    {
                        Id = (int)t.Id,
                        Language = t.Language,
                        ResourceId = (int)t.ResourceId,
                        ModificationDate = t.ModificationDate,
                        Value = t.Value
                    }).ToList()
                }).SingleOrDefault();

            result?.Translations.ForEach(t => t.LocalizationResource = result);

            return result;
        }


        /// <summary>
        /// Adds the translation for the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="translation">The translation.</param>
        /// <exception cref="ArgumentNullException">
        /// resource
        /// or
        /// translation
        /// </exception>
        public void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (translation == null) throw new ArgumentNullException(nameof(translation));

            var context = GetDbContextInstance();

            var entity = new LocalizationResourceTranslationEntity
            {
                Language = translation.Language,
                ResourceId = translation.ResourceId,
                Value = translation.Value,
                ModificationDate = translation.ModificationDate
            };
            context.Add(entity);

            context.SaveChanges();
        }

        /// <summary>
        /// Updates the translation for the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="translation">The translation.</param>
        /// <exception cref="ArgumentNullException">
        /// resource
        /// or
        /// translation
        /// </exception>
        public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (translation == null) throw new ArgumentNullException(nameof(translation));

            var context = GetDbContextInstance();

            var entity = context.Find<LocalizationResourceTranslationEntity>(translation.Id);
            if (entity != null)
            {
                entity.Value = translation.Value;
                entity.ModificationDate = translation.ModificationDate;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the translation.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="translation">The translation.</param>
        /// <exception cref="ArgumentNullException">
        /// resource
        /// or
        /// translation
        /// </exception>
        public void DeleteTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (translation == null) throw new ArgumentNullException(nameof(translation));

            var context = GetDbContextInstance();

            var entity = context.Find<LocalizationResourceTranslationEntity>(translation.Id);
            if (entity != null)
            {
                context.Remove(entity);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void UpdateResource(LocalizationResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var context = GetDbContextInstance();

            var entity = context.Find<LocalizationResourceEntity>();
            if (entity != null)
            {
                entity.ModificationDate = resource.ModificationDate;
                if (resource.IsModified != null)
                    entity.IsModified = resource.IsModified.Value;
                entity.Notes = resource.Notes;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void DeleteResource(LocalizationResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var context = GetDbContextInstance();

            var entity = context.Find<LocalizationResourceEntity>(resource.Id);

            if (entity != null)
            {
                context.Remove(entity);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes all resources. DANGEROUS!
        /// </summary>
        public void DeleteAllResources()
        {
            var context = GetDbContextInstance();

            context.RemoveRange(context.Set<LocalizationResourceEntity>());

            context.SaveChanges();
        }

        /// <summary>
        /// Inserts the resource in database.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void InsertResource(LocalizationResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var context = GetDbContextInstance();

            var entity = new LocalizationResourceEntity
            {
                ResourceKey = resource.ResourceKey,
                Author = resource.Author,
                FromCode = resource.FromCode,
                IsHidden = resource.IsHidden ?? false,
                IsModified = resource.IsModified ?? false,
                ModificationDate = resource.ModificationDate,
                Notes = resource.Notes,
                Translations = new List<LocalizationResourceTranslationEntity>()
            };

            resource.Translations.ForEach(translation =>
            {
                var item = new LocalizationResourceTranslationEntity
                {
                    Language = translation.Language,
                    Value = translation.Value,
                    ModificationDate = translation.ModificationDate
                };
                entity.Translations.Add(item);
            });

            context.Add(entity);

            context.SaveChanges();
        }

        /// <summary>
        /// Gets the available languages (reads in which languages translations are added).
        /// </summary>
        /// <param name="includeInvariant">if set to <c>true</c> ""include invariant"".</param>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
        {
            var context = GetDbContextInstance();

            var result = context.Set<LocalizationResourceTranslationEntity>()
                .AsNoTracking()
                .Where(p => p.Language != string.Empty)
                .Distinct()
                .Select(p => new CultureInfo(p.Language))
                .ToList();

            if (includeInvariant)
                result.Insert(0, CultureInfo.InvariantCulture);

            return result;
        }
    }
}
