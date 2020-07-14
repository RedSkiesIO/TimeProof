using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class PaymentMethodResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "paymentCustomerId")]
        public string PaymentCustomerId { get; set; }

        [JsonProperty(PropertyName = "card")]
        public CardResponse Card { get; set; }

        [JsonProperty(PropertyName = "address")]
        public AddressResponse Address { get; set; }
    }
}
