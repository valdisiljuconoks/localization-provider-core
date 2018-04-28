using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    [JsonObject]
    public class CreateOrUpdateTranslationRequestModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("translation")]
        public string Translation { get; set; }
    }
}
