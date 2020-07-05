// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbStringLocalizer : IStringLocalizer
    {
        private readonly CultureInfo _culture;

        public DbStringLocalizer()
        {

        }

        public DbStringLocalizer(CultureInfo culture) : this()
        {
            _culture = culture;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var values = _culture != null ? LocalizationProvider.Current.GetStringsByCulture(_culture) : LocalizationProvider.Current.GetStringsByCulture(CultureInfo.CurrentUICulture);

            return values.Select(value => new LocalizedString(value.Key, value.Value??value.Key, value.Value == null));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new DbStringLocalizer(culture);
        }

        LocalizedString IStringLocalizer.this[string name]
        {
            get
            {
                var value = _culture != null ? LocalizationProvider.Current.GetStringByCulture(name, _culture) : LocalizationProvider.Current.GetStringByCulture(name, CultureInfo.CurrentUICulture);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        LocalizedString IStringLocalizer.this[string name, params object[] arguments]
        {
            get
            {
                var value = _culture != null ? LocalizationProvider.Current.GetStringByCulture(name, _culture, arguments) : LocalizationProvider.Current.GetStringByCulture(name, CultureInfo.CurrentUICulture, arguments);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
    }
}
