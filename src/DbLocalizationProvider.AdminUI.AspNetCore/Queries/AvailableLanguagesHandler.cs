// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Queries
{
    public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
    {
        private readonly IList<CultureInfo> _supportedLanguages;

        public AvailableLanguagesHandler(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            if(localizationOptions == null)
                throw new ArgumentNullException(nameof(localizationOptions));

            _supportedLanguages = localizationOptions.Value.SupportedUICultures;
        }

        public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
        {
            return _supportedLanguages;
        }
    }
}
