using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class PaymentIntentResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "clientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public long Amount { get; set; }
    }
}