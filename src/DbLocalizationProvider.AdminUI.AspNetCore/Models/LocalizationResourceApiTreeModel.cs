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

using System.Collections.Generic;
using System.Globalization;
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

        private List<JObject> ConvertToApiModel(List<LocalizationResource> resources)
        {
            return new List<JObject>
                   {
                       new JObject
                       {
                           ["resourceKey"] = "MyNamespace",
                           ["_classes"] = new JObject
                                          {
                                              ["row"] = new JObject
                                                        {
                                                            ["parent-row"] = true
                                                        }
                                          },
                           ["_children"] = new JArray(
                                                      new JObject
                                                      {
                                                          ["resourceKey"] = "MyProperty",
                                                          ["translation"] = "Invariant Translation",
                                                          ["translation-en"] = "English Translation",
                                                          ["translation-no"] = null
                                                      },
                                                      new JObject
                                                      {
                                                          ["resourceKey"] = "MyProperty2",
                                                          ["translation"] = "Invariant Translation (2)",
                                                          ["translation-en"] = "English Translation (2)",
                                                          ["translation-no"] = null
                                                      })
                       }
                   };
        }
    }
}




/*
 *
 *
 *             rows: [
                {
                    resourceKey: 'MyNamespace',
                    _classes: {
                        row: { 'parent-row': true }
                    },
                    _children: [
                        {
                            resourceKey: 'MyClassName',
                            _children: [
                                {
                                    resourceKey: 'MyProperty',
                                    translationEn: 'English Translation',
                                },
                                {
                                    resourceKey: 'MyProperty2',
                                    translationEn: null,
                                },
                                {
                                    resourceKey: 'EmptyEnglish',
                                    translationEn: '',
                                },
                                {
                                    resourceKey: 'AnotherResource',
                                    translationEn: "Some translation",
                                    translationNo: "Tekst pa norsk",
                                }
                            ]
                        }
                    ]
                },
                {
                    resourceKey: 'MyNaspace2',
                    _children: [
                        {
                            resourceKey: 'MyClassName2',
                            _children: [
                                {
                                    resourceKey: 'MyProperty2',
                                    translationEn: 'English Translation',
                                },
                                {
                                    resourceKey: 'MyProperty22',
                                    translationEn: null,
                                },
                                {
                                    resourceKey: 'EmptyEnglish2',
                                    translationEn: '',
                                },
                                {
                                    resourceKey: 'AnotherResource2',
                                    translationEn: "Some translation",
                                    translationNo: "Tekst pa norsk",
                                }
                            ]
                        }
                    ]
                }
            ],
 *
 *
 */
