// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Service for providing localized strings
    /// </summary>
    public class DbStringLocalizer : IStringLocalizer
    {
        private readonly CultureInfo _culture;

        /// <summary>
        /// Creates new instance
        /// </summary>
        public DbStringLocalizer() { }

        /// <summary>
        /// Creates new instance specifying culture to use
        /// </summary>
        /// <param name="culture"></param>
        public DbStringLocalizer(CultureInfo culture) : this()
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
            var values = LocalizationProvider.Current.GetStringsByCulture(_culture ?? CultureInfo.CurrentUICulture);

            return values.Select(value => new LocalizedString(value.Key, value.Value ?? value.Key, value.Value == null));
        }

        /// <summary>
        /// Returns new instance of localizer with specified culture
        /// </summary>
        /// <param name="culture">Culture to use</param>
        /// <returns>Instance of localizer with specified culture</returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new DbStringLocalizer(culture);
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
                var value = _culture != null
                    ? LocalizationProvider.Current.GetStringByCulture(name, _culture)
                    : LocalizationProvider.Current.GetStringByCulture(name, CultureInfo.CurrentUICulture);
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
                var value = _culture != null
                    ? LocalizationProvider.Current.GetStringByCulture(name, _culture, arguments)
                    : LocalizationProvider.Current.GetStringByCulture(name, CultureInfo.CurrentUICulture, arguments);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
    }
}
