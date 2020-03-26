using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class SetupIntentDao : DaoBase
    {
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "clientSecret")]
        public string ClientSecret { get; set; }
    }
}
