using System;
using System.Collections.Generic;
using System.Text;

namespace AtlasCity.TimeProof.Abstractions
{
    public class EthSettings
    {
        public int GasPrice { get; set; }
        public string SecretKey { get; set; }
        public string ToAddress { get; set; }
        public string Network { get; set; }
    }
}
