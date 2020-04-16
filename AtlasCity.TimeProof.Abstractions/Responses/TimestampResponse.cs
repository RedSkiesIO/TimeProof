using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public class TimestampResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "txId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty(PropertyName = "txHash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "blockNumber")]
        public long BlockNumber { get; set; }

        [JsonProperty(PropertyName = "fileHash")]
        public string FileHash { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; set; }

        [JsonProperty(PropertyName = "network")]
        public string Network { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
