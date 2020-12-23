// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbHtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly ExpressionHelper _expressionHelper;
        private readonly CultureInfo _language;
        private readonly ILocalizationProvider _localizationProvider;
        private readonly DbStringLocalizerFactory _localizerFactory;

        public DbHtmlLocalizerFactory(
            DbStringLocalizerFactory localizerFactory,
            ILocalizationProvider localizationProvider,
            ExpressionHelper expressionHelper)
        {
            _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            _expressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
        }

        private DbHtmlLocalizerFactory(
            CultureInfo language,
            DbStringLocalizerFactory localizerFactory,
            ILocalizationProvider localizationProvider,
            ExpressionHelper expressionHelper) : this(localizerFactory, localizationProvider, expressionHelper)
        {
            _language = language ?? throw new ArgumentNullException(nameof(language));
        }

        public IHtmlLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return new DbHtmlLocalizer(_localizerFactory.Create(baseName, location), _expressionHelper);
        }

        public IHtmlLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            return new DbHtmlLocalizer(_localizerFactory.Create(resourceSource), _expressionHelper);
        }

        public DbHtmlLocalizerFactory ChangeLanguage(CultureInfo language)
        {
            return new DbHtmlLocalizerFactory(language,
                                              _localizerFactory.ChangeLanguage(language),
                                              _localizationProvider,
                                              _expressionHelper);
        }
    }
}
