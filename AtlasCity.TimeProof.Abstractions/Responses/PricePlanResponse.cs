using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class PricePlanResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public long Price { get; set; }

        [JsonProperty(PropertyName = "noOfStamps")]
        public int NoOfStamps { get; set; }

        [JsonProperty(PropertyName = "freqDesc")]
        public string FreqDesc { get; set; }
    }
}