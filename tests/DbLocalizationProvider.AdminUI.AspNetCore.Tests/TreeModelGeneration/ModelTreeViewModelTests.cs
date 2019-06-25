using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests.TreeModelGeneration
{
    public class ModelTreeViewModelTests
    {
        private const string _segmentPropertyName = "resourceKey";

        [Fact]
        public void GenerateSampleTreeModel_SingleResource()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("This.Is.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian"
                                                       }
                                                   }
                                }
                            };

            var languages = new [] { new CultureInfo("en"), new CultureInfo("lv") };

            var sut = new LocalizationResourceApiTreeModel(resources, languages, 100, 100);

            var result = sut.ConvertToApiModel(resources);
            var first = result.Single();

            Assert.NotNull(first);
            Assert.Equal("This", first[_segmentPropertyName]);
            Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
        }

        [Fact]
        public void GenerateSampleTreeModel_SingleResource_NoSplitInKey()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("ThisIsResourceKey")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian"
                                                       }
                                                   }
                                }
                            };

            var languages = new [] { new CultureInfo("en"), new CultureInfo("lv") };

            var sut = new LocalizationResourceApiTreeModel(resources, languages, 100, 100);

            var result = sut.ConvertToApiModel(resources);
            var first = result.Single();

            Assert.NotNull(first);
            Assert.Equal("ThisIsResourceKey", first[_segmentPropertyName]);
        }

        [Fact]
        public void GenerateSampleTreeModel_SingleResource_MissingTranslation()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("This.Is.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian"
                                                       }
                                                   }
                                }
                            };

            var languages = new [] { new CultureInfo("en"), new CultureInfo("lv"), new CultureInfo("no"),  };

            var sut = new LocalizationResourceApiTreeModel(resources, languages, 100, 100);

            var result = sut.ConvertToApiModel(resources);
            var first = result.Single();

            Assert.NotNull(first);
            Assert.Equal("This", first[_segmentPropertyName]);
            Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
            Assert.Null(((JValue)result[0]["_children"][0]["_children"][0]["_children"][0]["translation-no"]).Value);
        }

        [Fact]
        public void GenerateSampleTreeModel_MultipleResources_NoSharedKeyRoots()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("This.Is.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian"
                                                       }
                                                   }
                                },
                                new LocalizationResource("Another.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant 2"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English 2"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian 2"
                                                       }
                                                   }
                                }
                            };

            var languages = new [] { new CultureInfo("en"), new CultureInfo("lv") };

            var sut = new LocalizationResourceApiTreeModel(resources, languages, 100, 100);

            var result = sut.ConvertToApiModel(resources);
            var first = result.First();
            var second = result[1];

            Assert.NotNull(first);
            Assert.Equal("Another", second[_segmentPropertyName]);
            Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
            Assert.Equal("Invariant 2", result[1]["_children"][0]["_children"][0]["translation"]);
        }

        [Fact]
        public void GenerateSampleTreeModel_MultipleResources_SharedKeyRoots()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("This.Is.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian"
                                                       }
                                                   }
                                },
                                new LocalizationResource("This.Is.Resource.AnotherKey")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation      // invariant
                                                       {
                                                           Language = "",
                                                           Value = "Invariant 2"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "English 2"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "lv",
                                                           Value = "Latvian 2"
                                                       }
                                                   }
                                }
                            };

            var languages = new [] { new CultureInfo("en"), new CultureInfo("lv") };

            var sut = new LocalizationResourceApiTreeModel(resources, languages, 100, 100);

            var result = sut.ConvertToApiModel(resources);
            var first = result.First();

            Assert.NotNull(first);
            Assert.Equal("Invariant", result[0]["_children"][0]["_children"][0]["_children"][0]["translation"]);
            Assert.Equal("Invariant 2", result[0]["_children"][0]["_children"][0]["_children"][1]["translation"]);
        }
    }
}
