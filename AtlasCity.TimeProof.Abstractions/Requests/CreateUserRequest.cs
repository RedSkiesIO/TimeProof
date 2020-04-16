using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public class CreateUserRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

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
        public AddressRequest Address { get; set; }

        [JsonProperty(PropertyName = "pricePlanId")]
        public string PricePlanId { get; set; }

        [JsonProperty(PropertyName = "paymentIntentId")]
        public string PaymentIntentId { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}".TrimEnd(" ".ToCharArray());
            }
        }

        public DateTime MembershipStartDate { get; set; }
    }
}