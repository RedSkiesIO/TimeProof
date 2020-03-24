using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PricePlanDao: DaoBase
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public long Price { get; set; }

        [JsonProperty(PropertyName = "noofstamps")]
        public long NoOfStamps { get; set; }
    }
}
