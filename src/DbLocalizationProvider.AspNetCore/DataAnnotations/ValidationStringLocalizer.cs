// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    public class ValidationStringLocalizer : IStringLocalizer
    {
        private readonly Type _containerType;
        private readonly CultureInfo _culture;
        private readonly string _propertyName;
        private readonly ValidationAttribute _validatorMetadata;

        public ValidationStringLocalizer(Type containerType, string propertyName, ValidationAttribute validatorMetadata) : this(containerType,
                                                                                                                                propertyName,
                                                                                                                                validatorMetadata,
                                                                                                                                CultureInfo.CurrentUICulture) { }

        private ValidationStringLocalizer(Type containerType, string propertyName, ValidationAttribute validatorMetadata, CultureInfo culture)
        {
            _containerType = containerType;
            _propertyName = propertyName;
            _validatorMetadata = validatorMetadata;
            _culture = culture;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new ValidationStringLocalizer(_containerType, _propertyName, _validatorMetadata, culture);
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = LocalizationProvider.Current.GetString(ResourceKeyBuilder.BuildResourceKey(_containerType, _propertyName, _validatorMetadata));
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var value = LocalizationProvider.Current.GetStringByCulture(ResourceKeyBuilder.BuildResourceKey(_containerType, _propertyName, _validatorMetadata),
                                                                            _culture,
                                                                            arguments);

                return new LocalizedString(name, value ?? name, value == null);
            }
        }
    }
}
