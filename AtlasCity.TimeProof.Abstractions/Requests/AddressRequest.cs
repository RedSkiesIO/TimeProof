using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public class AddressRequest
    {
        [JsonProperty(PropertyName = "line1")]
        public string Line1 { get; set; }

        [JsonProperty(PropertyName = "line2")]
        public string Line2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }
}
