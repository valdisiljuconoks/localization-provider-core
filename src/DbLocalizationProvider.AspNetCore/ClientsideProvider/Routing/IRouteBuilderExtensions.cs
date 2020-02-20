// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing
{
    public static class IRouteBuilderExtensions
    {
        public static IRouteBuilder MapDbLocalizationClientsideProvider(this IRouteBuilder builder, string path = "/jsl10n")
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            ClientsideConfigurationContext.SetRootPath(path);

            builder.MapMiddlewareRoute(path + "/{*remaining}", b => b.UseMiddleware<RequestHandler>());

            return builder;
        }
    }
}
