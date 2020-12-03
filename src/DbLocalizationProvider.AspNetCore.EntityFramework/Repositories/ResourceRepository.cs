// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AspNetCore.EntityFramework.Entities;
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
            //if (resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            //using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            //{
            //    conn.Open();

            //    var strCmd = @"SELECT
            //            r.""Id"",
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
            //        FROM public.""LocalizationResources"" r
            //        LEFT JOIN public.""LocalizationResourceTranslations"" t ON r.""Id"" = t.""ResourceId""
            //        WHERE ""ResourceKey"" = @Key;";

            //    var cmd = new NpgsqlCommand(strCmd, conn);
            //    cmd.Parameters.AddWithValue("Key", resourceKey);

            //    var reader = cmd.ExecuteReader();

            //    if (!reader.Read()) return null;

            //    var result = CreateResourceFromSqlReader(resourceKey, reader);

            //    // read 1st translation
            //    // if TranslationId is NULL - there is no translations for given resource
            //    if (!reader.IsDBNull(reader.GetOrdinal("TranslationId")))
            //    {
            //        result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
            //        while (reader.Read()) result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
            //    }

            //    return result;
            //}
            return null;
        }

        //private LocalizationResource CreateResourceFromSqlReader(string key, NpgsqlDataReader reader)
        //{
        //    return new LocalizationResource(key)
        //    {
        //        Id = reader.GetInt32(reader.GetOrdinal(nameof(LocalizationResource.Id))),
        //        Author = reader.GetStringSafe(nameof(LocalizationResource.Author)) ?? "unknown",
        //        FromCode = reader.GetBooleanSafe(nameof(LocalizationResource.FromCode)),
        //        IsHidden = reader.GetBooleanSafe(nameof(LocalizationResource.IsHidden)),
        //        IsModified = reader.GetBooleanSafe(nameof(LocalizationResource.IsModified)),
        //        ModificationDate = reader.GetDateTime(reader.GetOrdinal(nameof(LocalizationResource.ModificationDate))),
        //        Notes = reader.GetStringSafe(nameof(LocalizationResource.Notes))
        //    };
        //}

        //private LocalizationResourceTranslation CreateTranslationFromSqlReader(NpgsqlDataReader reader,
        //    LocalizationResource result)
        //{
        //    return new LocalizationResourceTranslation
        //    {
        //        Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
        //        ResourceId = result.Id,
        //        Value = reader.GetStringSafe("Translation"),
        //        Language = reader.GetStringSafe("Language") ?? string.Empty,
        //        ModificationDate = reader.GetDateTime(reader.GetOrdinal("TranslationModificationDate")),
        //        LocalizationResource = result
        //    };
        //}

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
            //if (resource == null) throw new ArgumentNullException(nameof(resource));

            //using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            //{
            //    conn.Open();

            //    var cmd = new NpgsqlCommand(
            //        @"INSERT INTO public.""LocalizationResources"" (""ResourceKey"", ""Author"", ""FromCode"", ""IsHidden"", ""IsModified"", ""ModificationDate"", ""Notes"") OUTPUT INSERTED.ID VALUES (@resourceKey, @author, @fromCode, @isHidden, @isModified, @modificationDate, @notes)",
            //        conn);

            //    cmd.Parameters.AddWithValue("resourceKey", resource.ResourceKey);
            //    cmd.Parameters.AddWithValue("author", resource.Author ?? "unknown");
            //    cmd.Parameters.AddWithValue("fromCode", resource.FromCode);
            //    cmd.Parameters.AddWithValue("isHidden", resource.IsHidden);
            //    cmd.Parameters.AddWithValue("isModified", resource.IsModified);
            //    cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);
            //    cmd.Parameters.AddSafeWithValue("notes", resource.Notes);

            //    // get inserted resource ID
            //    var resourcePk = (int) cmd.ExecuteScalar();

            //    // if there are also provided translations - execute those in the same connection also
            //    if (resource.Translations.Any())
            //        foreach (var translation in resource.Translations)
            //        {
            //            cmd = new NpgsqlCommand(
            //                @"INSERT INTO public.""LocalizationResourceTranslations"" (""Language"", ""ResourceId"", ""Value"", ""ModificationDate"") VALUES (@language, @resourceId, @translation, @modificationDate)",
            //                conn);
            //            cmd.Parameters.AddWithValue("language", translation.Language);
            //            cmd.Parameters.AddWithValue("resourceId", resourcePk);
            //            cmd.Parameters.AddWithValue("translation", translation.Value);
            //            cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);

            //            cmd.ExecuteNonQuery();
            //        }
            //}
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
