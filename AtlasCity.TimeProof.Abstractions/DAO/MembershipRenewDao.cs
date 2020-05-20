using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PendingMembershipChangeDao : DaoBase
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "newPricePlanId")]
        public string NewPricePlanId { get; set; }

        [JsonProperty(PropertyName = "renewEpoch")]
        public int RenewEpoch { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
