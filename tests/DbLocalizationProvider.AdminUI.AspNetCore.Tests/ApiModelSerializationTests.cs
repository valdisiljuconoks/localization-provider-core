using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using Newtonsoft.Json;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests
{
    public class ApiModelSerializationTests
    {
        [Fact]
        public void SearializeSimpleResourceWith2Languages()
        {
            var model = new LocalizationResourceApiModel(new List<LocalizationResource>
                                                         {
                                                             new LocalizationResource("the-key1")
                                                             {
                                                                 Translations = new List<LocalizationResourceTranslation>
                                                                                {
                                                                                    new LocalizationResourceTranslation
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
                                                                                        Language = "no",
                                                                                        Value = "Norsk"
                                                                                    }
                                                                                }
                                                             }
                                                         },
                                                         new List<CultureInfo>
                                                         {
                                                             new CultureInfo("en"),
                                                             new CultureInfo("no")
                                                         },
                                                         120,
                                                         80);

            var result = JsonConvert.SerializeObject(model);
        }

        [Fact]
        public void SearializeSimpleResourceWith2Languages_MissingTranslationFor1OfThem()
        {
            var model = new LocalizationResourceApiModel(new List<LocalizationResource>
                                                         {
                                                             new LocalizationResource("the-key1")
                                                             {
                                                                 Translations = new List<LocalizationResourceTranslation>
                                                                                {
                                                                                    new LocalizationResourceTranslation
                                                                                    {
                                                                                        Language = "",
                                                                                        Value = "Invariant"
                                                                                    },
                                                                                    new LocalizationResourceTranslation
                                                                                    {
                                                                                        Language = "en",
                                                                                        Value = "English"
                                                                                    }
                                                                                }
                                                             }
                                                         },
                                                         new List<CultureInfo>
                                                         {
                                                             new CultureInfo("en"),
                                                             new CultureInfo("no")
                                                         },
                                                         120,
                                                         80);

            var result = JsonConvert.SerializeObject(model);
        }
    }
}
