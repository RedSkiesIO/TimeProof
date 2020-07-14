using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class InvoiceNumberDao : DaoBase
    {
        [JsonProperty(PropertyName = "number")]
        public long Number { get; set; }
    }
}