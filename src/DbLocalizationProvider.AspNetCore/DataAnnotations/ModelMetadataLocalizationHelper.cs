// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    public class ModelMetadataLocalizationHelper
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly ResourceKeyBuilder _keyBuilder;

        public ModelMetadataLocalizationHelper(ILocalizationProvider localizationProvider, ResourceKeyBuilder keyBuilder)
        {
            _localizationProvider = localizationProvider;
            _keyBuilder = keyBuilder;
        }

        internal string GetTranslation(string resourceKey)
        {
            var result = resourceKey;
            if (!ConfigurationContext.Current.EnableLocalization())  return result;

            var localizedDisplayName = _localizationProvider.GetString(resourceKey);
            result = localizedDisplayName;

            // for the legacy purposes - we need to look for this resource translation using display name
            // once again - this will make sure that existing XPath resources are still working
            if (localizedDisplayName != null && !ConfigurationContext.Current.ResourceLookupFilter(localizedDisplayName))
            {
                result = _localizationProvider.GetString(localizedDisplayName);
            }

            // If other data annotations exists except for [Display], an exception is thrown when display name is ""
            // It should be null to avoid exception as ModelMetadata.GetDisplayName only checks for null and not String.Empty
            return string.IsNullOrWhiteSpace(localizedDisplayName) ? null : result;
        }

        internal string GetTranslation(Type containerType, string propertyName)
        {
            var resourceKey = _keyBuilder.BuildResourceKey(containerType, propertyName);

            return GetTranslation(resourceKey);
        }
    }
}
