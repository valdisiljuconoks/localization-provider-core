// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Queries
{
    public class AvailableLanguagesHandler : IQueryHandler<AvailableLanguages.Query, IEnumerable<CultureInfo>>
    {
        private readonly IList<CultureInfo> _supportedLanguages;

        public AvailableLanguagesHandler(IList<CultureInfo> supportedLanguages)
        {
            _supportedLanguages = supportedLanguages ?? throw new ArgumentNullException(nameof(supportedLanguages));
        }

        public IEnumerable<CultureInfo> Execute(AvailableLanguages.Query query)
        {
            return _supportedLanguages;
        }
    }
}
