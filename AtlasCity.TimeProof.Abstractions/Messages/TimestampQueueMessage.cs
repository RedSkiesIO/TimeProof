using System;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Messages
{
    [Serializable]
    public class TimestampQueueMessage
    {
        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        public string PopReceipt { get; set; }

        [JsonProperty(PropertyName = "timestampId", Required = Required.Always)]
        public string TimestampId { get; set; }

        [JsonProperty(PropertyName = "transactionId", Required = Required.Always)]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "created", Required = Required.Always)]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "isPremiumPlan", Required = Required.Always)]
        public bool IsPremiumPlan { get; set; }
    }
}