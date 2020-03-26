using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class UserDao : DaoBase
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string PaymentCustomerId { get; set; }

        [JsonProperty(PropertyName = "setupIntentId")]
        public string SetupIntentId { get; set; }

        [JsonProperty(PropertyName = "address")]
        public AddressDao Address { get; set; }
    }
}
