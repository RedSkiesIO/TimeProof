using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class UserResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public AddressResponse Address { get; set; }

        [JsonProperty(PropertyName = "remainingTimeStamps")]
        public int RemainingTimeStamps { get; set; }

        [JsonProperty(PropertyName = "pricePlanId")]
        public string PricePlanId { get; set; }

        [JsonProperty(PropertyName = "paymentIntentId")]
        public string PaymentIntentId { get; set; }

        [JsonProperty(PropertyName = "membershipRenewDate")]
        public DateTime MembershipRenewDate { get; set; }

        [JsonProperty(PropertyName = "pendingPricePlanId")]
        public string PendingPricePlanId { get; set; }

        [JsonProperty(PropertyName = "keyEmailDate")]
        public DateTime KeyEmailDate { get; set; }
    }
}