// (c)Valdis Iljuconoks, Andriy S'omak. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore.Storage
{
    internal class Settings
    {
        // TODO: Need to be redesigned. There is stupid hack, cuz LocalizationProvider architecture is not compatible with DI at the moment
        public static IServiceCollection Services { get; set; }
        public static Type ContextType { get; set; }
    }
}