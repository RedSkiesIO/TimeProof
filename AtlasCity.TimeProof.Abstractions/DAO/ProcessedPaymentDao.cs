using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class ProcessedPaymentDao : DaoBase
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "paymentServiceId")]
        public string PaymentServiceId { get; set; }

        [JsonProperty(PropertyName = "email")]
        public long Amount { get; set; }

        [JsonProperty(PropertyName = "pricePlanId")]
        public string PricePlanId { get; set; }

        [JsonProperty(PropertyName = "noOfStamps")]
        public int NoOfStamps { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
    }
}
