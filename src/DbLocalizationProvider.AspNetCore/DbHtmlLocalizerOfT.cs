// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbHtmlLocalizer<TResource> : HtmlLocalizer<TResource>, ILocalizationServicesAccessor, ICultureAwareHtmlLocalizer
    {
        private readonly DbHtmlLocalizerFactory _factory;

        public DbHtmlLocalizer(DbHtmlLocalizerFactory factory, ExpressionHelper expressionHelper) : base(factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            ExpressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
        }

        public ExpressionHelper ExpressionHelper { get; }

        public override IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            return ChangeLanguage(culture);
        }

        public IHtmlLocalizer ChangeLanguage(CultureInfo language)
        {
            return new DbHtmlLocalizer<TResource>(_factory.ChangeLanguage(language), ExpressionHelper);
        }
    }
}
