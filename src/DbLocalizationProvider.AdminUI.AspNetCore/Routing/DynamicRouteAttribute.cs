// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Routing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class DynamicRouteAttribute : Attribute, IRouteTemplateProvider
{
    public string? Template
    {
        get
        {
            return "/localization-admin/api/service";
        }
    }

    public int? Order => -1000 + 10;

    public string? Name { get; set; }
}
