// Copyright (c) 2019 Valdis Iljuconoks.
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
using DbLocalizationProvider.AspNetCore;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class LocalizationResourceApiTreeModel : BaseApiModel
    {
        private const string _segmentPropertyName = "segmentKey";
        private readonly int _popupTitleLength;
        private readonly int _listDisplayLength;

        public LocalizationResourceApiTreeModel(
            List<LocalizationResource> resources,
            IEnumerable<CultureInfo> languages,
            int popupTitleLength,
            int listDisplayLength) : base(languages)
        {
            _popupTitleLength = popupTitleLength;
            _listDisplayLength = listDisplayLength;
            Resources = ConvertToApiModel(resources);
        }

        internal List<JObject> ConvertToApiModel(List<LocalizationResource> resources)
        {
            var result = new JArray();
            foreach(var resource in resources)
            {
                var segments = resource.ResourceKey.Split(new [] { '.', '+' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                void AddChildrenNodes(JArray children, List<string> list, LocalizationResource localizationResource, int currentLevel, int depth)
                {
                    while(true)
                    {
                        var (head, tail) = list;
                        var el = children.FirstOrDefault(c => c[_segmentPropertyName] != null && c[_segmentPropertyName].ToString().Equals(head, StringComparison.InvariantCultureIgnoreCase));
                        if(el == null)
                        {
                            el = new JObject { [_segmentPropertyName] = head };
                            if(currentLevel == depth)
                            {
                                // process leaf
                                var key = localizationResource.ResourceKey;
                                el["resourceKey"] = key;
                                el["displayKey"] = $"{key.Substring(0, key.Length > _listDisplayLength ? _listDisplayLength : key.Length)}{(key.Length > _listDisplayLength ? "..." : "")}";
                                el["titleKey"] = $"{(key.Length > _popupTitleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(_popupTitleLength, key.Length))}";
                                el["syncedFromCode"] = localizationResource.FromCode;
                                el["isModified"] = localizationResource.IsModified;
                                el["allowDelete"] = !localizationResource.FromCode;
                                el["translation"] = localizationResource.Translations.FindByLanguage(CultureInfo.InvariantCulture)?.Value;
                                foreach(var language in Languages)
                                    el["translation-" + language.Code] = localizationResource.Translations.FindByLanguage(language.Code)?.Value;
                            }
                            else
                            {
                                el["_children"] = new JArray();
                                el["_classes"] = new JObject { ["row"] = new JObject { ["parent-row"] = true } };
                            }

                            children.Add(el);
                        }

                        if(tail.Any())
                        {
                            children = el["_children"] as JArray;
                            list = tail;
                            currentLevel += 1;
                            continue;
                        }

                        break;
                    }
                }

                if(segments.Any())
                    AddChildrenNodes(result, segments, resource, 0, segments.Count - 1);
            }

            return result.Cast<JObject>().ToList();
        }
    }
}
