using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public abstract class DaoBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
