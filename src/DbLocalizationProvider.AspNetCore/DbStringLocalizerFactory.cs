// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ExpressionHelper _expressionHelper;
        private readonly CultureInfo _language;
        private readonly ILocalizationProvider _localizationProvider;

        public DbStringLocalizerFactory(ILocalizationProvider localizationProvider, ExpressionHelper expressionHelper)
        {
            _localizationProvider = localizationProvider;
            _expressionHelper = expressionHelper;
        }

        private DbStringLocalizerFactory(
            CultureInfo language,
            ILocalizationProvider localizationProvider,
            ExpressionHelper expressionHelper) : this(localizationProvider, expressionHelper)
        {
            _language = language;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new DbStringLocalizer(_language, _localizationProvider, _expressionHelper);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbStringLocalizer(_language, _localizationProvider, _expressionHelper);
        }

        public DbStringLocalizerFactory ChangeLanguage(CultureInfo language)
        {
            return new DbStringLocalizerFactory(language, _localizationProvider, _expressionHelper);
        }
    }
}
