using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class MembershipRenewDao : DaoBase
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "newPricePlanId")]
        public string NewPricePlanId { get; set; }

        [JsonProperty(PropertyName = "renewDate")]
        public DateTime MembershipRenewDate { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
