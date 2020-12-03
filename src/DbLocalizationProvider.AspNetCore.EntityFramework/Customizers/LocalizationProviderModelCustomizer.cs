// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DbLocalizationProvider.AspNetCore.EntityFramework.Customizers
{
    public class LocalizationProviderModelCustomizer : RelationalModelCustomizer
    {
        public LocalizationProviderModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies)
        {
        }

        public override void Customize(ModelBuilder builder, DbContext context)
        {
            builder.Entity<LocalizationResourceEntity>(entity =>
            {
                entity.ToTable("LocalizationResources");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Author)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(p => p.ResourceKey)
                    .HasMaxLength(1000)
                    .IsRequired();
                entity.Property(p => p.Notes)
                    .HasMaxLength(3000)
                    .IsRequired(false);
            });

            builder.Entity<LocalizationResourceTranslationEntity>(entity =>
            {
                entity.ToTable("LocalizationResourceTranslations");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Language)
                    .HasMaxLength(10)
                    .IsRequired();
                entity.Property(p => p.Value)
                    .IsRequired(false);
                entity.HasOne(p => p.Resource)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(p => p.ResourceId);
                entity.HasIndex(p => new { p.Language, p.ResourceId })
                    .IsUnique();
            });

            base.Customize(builder, context);
        }
    }
}
