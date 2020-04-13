using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class SetupIntentResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "clientSecret")]
        public string ClientSecret { get; set; }
    }
}
