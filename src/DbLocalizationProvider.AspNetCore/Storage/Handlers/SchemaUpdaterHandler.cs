// Copyright (c) Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.AspNetCore.Storage.Handlers
{
    public class SchemaUpdaterHandler : ICommandHandler<UpdateSchema.Command>
    {
        public void Execute(UpdateSchema.Command command)
        {
            // check db schema and update if needed
            EnsureDatabaseSchema();
        }

        private void EnsureDatabaseSchema()
        {
            // There is MOCK. Nothing to do for EntityFramework implementation
        }
    }
}
