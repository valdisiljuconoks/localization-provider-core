// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore
{
    public class LanguageEntities : DbContext
    {
        private readonly string _connectionString;

        public LanguageEntities() : this(ConfigurationContext.Current.DbContextConnectionString)
        {
        }

        public LanguageEntities(DbContextOptions<LanguageEntities> options) : base(options)
        {
        }

        public LanguageEntities(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<LocalizationResource> LocalizationResources { get; set; }

        public virtual DbSet<LocalizationResourceTranslation> LocalizationResourceTranslations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var resource = builder.Entity<LocalizationResource>();
            resource.HasKey(r => r.Id);
            resource.Property(r => r.ResourceKey)
                .IsRequired()
                .HasMaxLength(1700);

            resource.Property(r => r.Author)
                .HasMaxLength(100);

            resource.HasIndex(r => r.ResourceKey)
                .HasName("IX_ResourceKey")
                .IsUnique();

            resource.HasMany(r => r.Translations)
                .WithOne(t => t.LocalizationResource)
                .IsRequired(false);

            var translation = builder.Entity<LocalizationResourceTranslation>();
            translation.HasKey(t => t.Id);
            translation.HasOne(t => t.LocalizationResource)
                .WithMany(r => r.Translations)
                .HasForeignKey(t => t.ResourceId);

            translation.Property(t => t.ResourceId).IsRequired();
            translation.Property(t => t.Language)
                .HasMaxLength(10);
        }
    }
}
