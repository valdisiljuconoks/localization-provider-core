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
                                                          ["translationEn"] = "English Translation"
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
