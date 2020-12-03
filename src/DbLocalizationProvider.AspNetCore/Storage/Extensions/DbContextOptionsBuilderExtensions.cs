// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AspNetCore.Storage.Customizers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DbLocalizationProvider.AspNetCore.Storage.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseLocalizationProvider(this DbContextOptionsBuilder builder)
        {
            builder.ReplaceService<IModelCustomizer, LocalizationProviderModelCustomizer>();
            return builder;
        }
    }
}
