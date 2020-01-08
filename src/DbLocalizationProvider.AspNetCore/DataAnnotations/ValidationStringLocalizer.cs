// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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
