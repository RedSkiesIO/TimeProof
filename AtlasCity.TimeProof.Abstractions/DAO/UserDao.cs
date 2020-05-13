using System;
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

        [JsonProperty(PropertyName = "remainingTimeStamps")]
        public int RemainingTimeStamps { get; set; }

        [JsonProperty(PropertyName = "pricePlanId")]
        public string CurrentPricePlanId { get; set; }

        [JsonProperty(PropertyName = "renewPricePlanId")]
        public string RenewPricePlanId { get; set; }

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

        public DateTime MembershipRenewDate { get; set; }
    }
}