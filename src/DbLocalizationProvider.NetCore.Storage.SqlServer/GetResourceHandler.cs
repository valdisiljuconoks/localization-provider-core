// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.NetCore.Storage.SqlServer;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNetCore.Queries
{
    public class GetResourceHandler : IQueryHandler<GetResource.Query, LocalizationResource>
    {
        public LocalizationResource Execute(GetResource.Query query)
        {
            var repository = new ResourceRepository();

            return repository.GetByKey(query.ResourceKey);
        }
    }
}
