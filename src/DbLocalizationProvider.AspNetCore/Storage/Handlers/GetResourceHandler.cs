// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.Storage.Repositories;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNetCore.Storage.Handlers
{
    /// <summary>
    /// Retrieves single resource
    /// </summary>
    public class GetResourceHandler : IQueryHandler<GetResource.Query, LocalizationResource>
    {
        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public LocalizationResource Execute(GetResource.Query query)
        {
            var repository = new ResourceRepository();
            return repository.GetByKey(query.ResourceKey);
        }
    }
}
