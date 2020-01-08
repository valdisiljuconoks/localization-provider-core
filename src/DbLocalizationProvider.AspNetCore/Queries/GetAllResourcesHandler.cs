// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    public class GetAllResourcesHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            using(var db = new LanguageEntities())
            {
                return db.LocalizationResources.Include(r => r.Translations)
                    .AsNoTracking()
                    .ToList();
            }
        }
    }
}
