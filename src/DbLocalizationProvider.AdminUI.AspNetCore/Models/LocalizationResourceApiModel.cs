// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class LocalizationResourceApiModel : BaseApiModel
    {
        private readonly int _listDisplayLength;
        private readonly int _popupTitleLength;

        public LocalizationResourceApiModel(
            ICollection<LocalizationResource> resources,
            IEnumerable<CultureInfo> languages,
            int popupTitleLength,
            int listDisplayLength) : base(languages)
        {
            if(resources == null)
                throw new ArgumentNullException(nameof(resources));

            if(languages == null)
                throw new ArgumentNullException(nameof(languages));

            _popupTitleLength = popupTitleLength;
            _listDisplayLength = listDisplayLength;
            Resources = resources.Select(r => ConvertToApiModel(r, languages)).ToList();
        }

        private JObject ConvertToApiModel(LocalizationResource resource, IEnumerable<CultureInfo> languages)
        {
            var key = resource.ResourceKey;
            var result = new JObject
                         {
                             ["key"] = key,
                             ["displayKey"] = $"{key.Substring(0, key.Length > _listDisplayLength ? _listDisplayLength : key.Length)}{(key.Length > _listDisplayLength ? "..." : "")}",
                             ["titleKey"] = $"{(key.Length > _popupTitleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(_popupTitleLength, key.Length))}",
                             ["syncedFromCode"] = resource.FromCode,
                             ["isModified"] = resource.IsModified,
                             ["allowDelete"] = !resource.FromCode,
                             ["_"] = resource.Translations.FindByLanguage(CultureInfo.InvariantCulture)?.Value,
                             ["isHidden"] = resource.IsHidden ?? false
                         };

            foreach(var language in languages)
                result[language.Name] = resource.Translations.FindByLanguage(language)?.Value;

            return result;
        }
    }
}
