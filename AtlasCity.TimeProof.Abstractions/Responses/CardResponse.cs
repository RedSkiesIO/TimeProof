using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class CardResponse
    {
        [JsonProperty(PropertyName = "last4")]
        public string Last4 { get; set; }

        [JsonProperty(PropertyName = "brand")]
        public string Brand { get; set; }

        [JsonProperty(PropertyName = "expMonth")]
        public long ExpMonth { get; set; }

        [JsonProperty(PropertyName = "expYear")]
        public long ExpYear { get; set; }

        [JsonProperty(PropertyName = "issuer")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }
}
