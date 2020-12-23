// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Service for providing localized strings
    /// </summary>
    public class DbStringLocalizer : IStringLocalizer, ILocalizationServicesAccessor, ICultureAwareStringLocalizer
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly ExpressionHelper _expressionHelper;
        private readonly CultureInfo _culture;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="localizationProvider">Instance of localization provider</param>
        /// <param name="expressionHelper">ExpressionHelper to be used for walking lambdas</param>
        public DbStringLocalizer(ILocalizationProvider localizationProvider, ExpressionHelper expressionHelper)
        {
            _localizationProvider = localizationProvider;
            _expressionHelper = expressionHelper;
        }

        /// <summary>
        /// Creates new instance specifying culture to use
        /// </summary>
        /// <param name="culture">Language to be used in this localizer.</param>
        /// <param name="localizationProvider">Instance of localization provider</param>
        /// <param name="expressionHelper">ExpressionHelper to be used for walking lambdas</param>
        public DbStringLocalizer(
            CultureInfo culture,
            ILocalizationProvider localizationProvider,
            ExpressionHelper expressionHelper) : this(localizationProvider, expressionHelper)
        {
            _culture = culture;
        }

        /// <summary>
        /// Returns all strings
        /// </summary>
        /// <param name="includeParentCultures">Whether result should include parent cultures as well</param>
        /// <returns>List of localized strings</returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var values = _localizationProvider.GetStringsByCulture(_culture ?? CultureInfo.CurrentUICulture);

            return values.Select(value => new LocalizedString(value.Key, value.Value ?? value.Key, value.Value == null));
        }

        /// <summary>
        /// Returns new instance of localizer with specified culture
        /// </summary>
        /// <param name="culture">Culture to use</param>
        /// <returns>Instance of localizer with specified culture</returns>
        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return ChangeLanguage(culture);
        }

        /// <summary>
        /// Change language of the provider and returns string localizer with specified language.
        /// </summary>
        /// <param name="language">Language to change to.</param>
        /// <returns>Localizer with specified language.</returns>
        public IStringLocalizer ChangeLanguage(CultureInfo language)
        {
            return new DbStringLocalizer(language, _localizationProvider, _expressionHelper);
        }

        /// <summary>
        /// Returns localized string by name
        /// </summary>
        /// <param name="name">Name of the string to localize</param>
        /// <returns>Localized string by name</returns>
        LocalizedString IStringLocalizer.this[string name]
        {
            get
            {
                var value = _localizationProvider.GetStringByCulture(name, _culture ?? CultureInfo.CurrentUICulture);

                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        /// <summary>
        /// Returns localized string by name and some formatting arguments if any (placeholder values)
        /// </summary>
        /// <param name="name">Name of the string to localize</param>
        /// <param name="arguments">Formatting arguments if any (placeholder values)</param>
        /// <returns>Localized string by name and filled in placeholders (if any)</returns>
        LocalizedString IStringLocalizer.this[string name, params object[] arguments]
        {
            get
            {
                var value = _localizationProvider.GetStringByCulture(name, _culture ?? CultureInfo.CurrentUICulture, arguments);

                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        /// <summary>
        /// Expression helper to be used to walk lambdas
        /// </summary>
        public ExpressionHelper ExpressionHelper => _expressionHelper;
    }
}
