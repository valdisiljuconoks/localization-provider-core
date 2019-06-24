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
        public LocalizationResourceApiTreeModel(
            List<LocalizationResource> resources,
            IEnumerable<CultureInfo> languages,
            int configMaxResourceKeyPopupTitleLength,
            int configMaxResourceKeyDisplayLength) : base(languages)
        {
            Resources = ConvertToApiModel(resources);
        }

        internal List<JObject> ConvertToApiModel(List<LocalizationResource> resources)
        {
            var result = new JArray();
            foreach(var resource in resources)
            {
                var segments = resource.ResourceKey.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();

                void AddChildrenNodes(JArray children, List<string> list, LocalizationResource localizationResource, int currentLevel, int depth)
                {
                    var (head, tail) = list;
                    var el = children.FirstOrDefault(c => c["resourceKey"] != null && c["resourceKey"].ToString().Equals(head, StringComparison.InvariantCultureIgnoreCase));
                    if(el == null)
                    {
                        el = new JObject { ["resourceKey"] = head };
                        if(currentLevel == depth)
                        {
                            // process leaf
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
                        AddChildrenNodes(el["_children"] as JArray, tail, localizationResource, currentLevel + 1, depth);
                }

                if(segments.Any())
                    AddChildrenNodes(result, segments, resource, 0, segments.Count - 1);
            }

            return result.Cast<JObject>().ToList();
        }
    }
}
