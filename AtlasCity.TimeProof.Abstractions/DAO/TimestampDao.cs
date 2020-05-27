using System;
using AtlasCity.TimeProof.Abstractions.Enums;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class TimestampDao : DaoBase
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

        [JsonProperty(PropertyName = "status")]
        public TimestampState Status { get; set; }

        [JsonProperty(PropertyName = "network")]
        public string Network { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "txId")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "blockNumber")]
        public long BlockNumber { get; set; }

        [JsonProperty(PropertyName = "add")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; set; }
    }
}