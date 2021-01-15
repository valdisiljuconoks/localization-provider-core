using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.Models;
using Newtonsoft.Json;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests
{
    public class ApiModelSerializationTests
    {
        [Fact]
        public void SerializeSimpleResourceWith2Languages()
        {
            var model = new LocalizationResourceApiModel(
                new List<LocalizationResource>
                {
                    new LocalizationResource("the-key1", false)
                    {
                        Translations = new LocalizationResourceTranslationCollection(false)
                        {
                            new LocalizationResourceTranslation { Language = "", Value = "Invariant" },
                            new LocalizationResourceTranslation { Language = "en", Value = "English" },
                            new LocalizationResourceTranslation { Language = "no", Value = "Norsk" }
                        }
                    }
                },
                new List<CultureInfo> { new CultureInfo("en"), new CultureInfo("no") },
                120,
                80,
                new UiOptions());

            var result = JsonConvert.SerializeObject(model);
        }

        [Fact]
        public void SerializeSimpleResourceWith2Languages_MissingTranslationFor1OfThem()
        {
            var model = new LocalizationResourceApiModel(
                new List<LocalizationResource>
                {
                    new LocalizationResource("the-key1", false)
                    {
                        Translations = new LocalizationResourceTranslationCollection(false)
                        {
                            new LocalizationResourceTranslation { Language = "", Value = "Invariant" },
                            new LocalizationResourceTranslation { Language = "en", Value = "English" }
                        }
                    }
                },
                new List<CultureInfo> { new CultureInfo("en"), new CultureInfo("no") },
                120,
                80,
                new UiOptions());

            var result = JsonConvert.SerializeObject(model);
        }
    }
}
