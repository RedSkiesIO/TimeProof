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

        [JsonProperty(PropertyName = "txPrice")]
        public double TransactionPrice { get; set; }

        [JsonProperty(PropertyName = "gasPrice")]
        public double GasPrice { get; set; }

        [JsonProperty(PropertyName = "freqDesc")]
        public string PaymentFrquencyDescription { get; set; }
    }
}