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
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class LocalizationResourceApiModel
    {
        private readonly int _popupTitleLength;
        private readonly int _listDisplayLength;

        public LocalizationResourceApiModel(ICollection<LocalizationResource> resources, IEnumerable<CultureInfo> languages, int popupTitleLength, int listDisplayLength)
        {
            if(resources == null)
                throw new ArgumentNullException(nameof(resources));

            if(languages == null)
                throw new ArgumentNullException(nameof(languages));

            _popupTitleLength = popupTitleLength;
            _listDisplayLength = listDisplayLength;
            Resources = resources.Select(r => ConvertToApiModel(r, languages)).ToList();
            Languages = languages.Select(l => new CultureApiModel(l.Name, l.EnglishName));
            Options = new UiOptions();
        }

        public List<JObject> Resources { get; }

        public IEnumerable<CultureApiModel> Languages { get; }

        public UiOptions Options { get; }

        private JObject ConvertToApiModel(LocalizationResource resource, IEnumerable<CultureInfo> languages)
        {
            var key = resource.ResourceKey;
            var result = new JObject
                         {
                             ["key"] = resource.ResourceKey,
                             ["displayKey"] = $"{key.Substring(0, key.Length > _listDisplayLength ? _listDisplayLength : key.Length)}{(key.Length > _listDisplayLength ? "..." : "")}",
                             ["titleKey"] = $"{(key.Length > _popupTitleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(_popupTitleLength, key.Length))}",
                             ["syncedFromCode"] = resource.FromCode,
                             ["isModified"] = resource.IsModified,
                             ["allowDelete"] = !resource.FromCode,
                             ["_"] = resource.Translations.FindByLanguage(CultureInfo.InvariantCulture)?.Value,
                             ["isHidden"] = (resource.IsHidden ?? false)
                         };

            foreach(var language in languages)
                result[language.Name] = resource.Translations.FindByLanguage(language)?.Value;

            return result;
        }
    }
}
