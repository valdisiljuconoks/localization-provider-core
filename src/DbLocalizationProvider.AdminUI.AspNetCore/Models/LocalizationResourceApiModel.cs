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
        public LocalizationResourceApiModel(ICollection<LocalizationResource> resources, IEnumerable<CultureInfo> languages)
        {
            if(resources == null)
                throw new ArgumentNullException(nameof(resources));

            if(languages == null)
                throw new ArgumentNullException(nameof(languages));

            Resources = resources.Select(r => ConvertToApiModel(r, languages)).ToList();
            Languages = languages.Select(l => new CultureApiModel(l.Name, l.EnglishName));
        }

        public List<JObject> Resources { get; }

        public IEnumerable<CultureApiModel> Languages { get; }

        public bool AdminMode { get; set; }

        private static JObject ConvertToApiModel(LocalizationResource resource, IEnumerable<CultureInfo> languages)
        {
            var displayLength = 120;
            var titleLength = 80;

            var key = resource.ResourceKey;

            var result = new JObject
                         {
                             ["key"] = resource.ResourceKey,
                             ["displayKey"] = $"{key.Substring(0, key.Length > displayLength ? displayLength : key.Length)}{(key.Length > displayLength ? "..." : "")}",
                             ["titleKey"] = $"{(key.Length > titleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(titleLength, key.Length))}",
                             ["syncedFromCode"] = resource.FromCode,
                             ["allowDelete"] = !resource.FromCode,
                             ["_"] = resource.Translations.FindByLanguage(CultureInfo.InvariantCulture)?.Value
                         };

            foreach(var language in languages)
                result[language.Name] = resource.Translations.FindByLanguage(language)?.Value;

            return result;
        }
    }
}
