using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public class CreateTimestampRequest
    {
        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "fileHash")]
        public string FileHash { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
