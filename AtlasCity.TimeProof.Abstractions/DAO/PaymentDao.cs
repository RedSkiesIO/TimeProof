using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PaymentDao
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "paymentMethodId")]
        public string PaymentMethodId { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public long Amount { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}