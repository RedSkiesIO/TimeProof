using Newtonsoft.Json;
using System;

namespace AtlasCity.TimeProof.Abstractions.EthResponse
{
    public class EthGasStationPrice
    {
        [JsonProperty(PropertyName = "fast")]
        public string Fast { get; set; }

        [JsonProperty(PropertyName = "average")]
        public string Average { get; set; }

        [JsonProperty(PropertyName = "safeLow")]
        public string SafeLow { get; set; }

        [JsonProperty(PropertyName = "fastWait")]
        public string FastWait { get; set; }

        [JsonProperty(PropertyName = "avgWait")]
        public string AverageWait { get; set; }

        [JsonProperty(PropertyName = "safeLowWait")]
        public string SafeLowWait { get; set; }

        [JsonIgnore]
        public double FastGwei
        {
            get
            {
                if (double.TryParse(Fast, out double parsedValue))
                {
                    if (parsedValue >= 10)
                        return parsedValue / 10;
                }

                return 0;
            }
        }

        [JsonIgnore]
        public double AverageGwei
        {
            get
            {
                if (double.TryParse(Average, out double parsedValue))
                {
                    if (parsedValue >= 10)
                        return parsedValue / 10;

                }
                return 0;
            }
        }

        [JsonIgnore]
        public double SafeLowGwei
        {
            get
            {
                if (double.TryParse(SafeLow, out double parsedValue))
                {
                    if (parsedValue >= 10)
                        return parsedValue / 10;

                }
                return 0;
            }
        }

        [JsonIgnore]
        public TimeSpan FastWaitTime
        {
            get
            {
                return new TimeSpan(0, ParserInt(FastWait), 0);
            }
        }

        [JsonIgnore]
        public TimeSpan AverageWaitTime
        {
            get
            {
                return new TimeSpan(0, ParserInt(AverageWait), 0);
            }
        }

        [JsonIgnore]
        public TimeSpan SafeLowWaitTime
        {
            get
            {
                return new TimeSpan(0, ParserInt(SafeLowWait), 0);
            }
        }

        private int ParserInt(string intString)
        {
            if (!string.IsNullOrWhiteSpace(intString) && int.TryParse(intString, out int parsedValue))
            {
                return parsedValue;
            }

            return 0;
        }
    }
}
