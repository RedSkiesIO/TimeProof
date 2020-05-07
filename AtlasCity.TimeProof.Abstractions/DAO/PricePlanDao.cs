using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PricePlanDao : DaoBase
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public long Price { get; set; }

        [JsonProperty(PropertyName = "noOfStamps")]
        public int NoOfStamps { get; set; }

        [JsonProperty(PropertyName = "gasPrice")]
        public int GasPrice { get; set; }

        [JsonProperty(PropertyName = "confirmDesc")]
        public string ConfirmationDescription { get; set; }
    }
}