using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class KeyDao : DaoBase
    {
      
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string KeyDetails { get; set; }
    }
}