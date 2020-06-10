using System;

namespace AtlasCity.TimeProof.Abstractions.EthResponse
{
    public class EthCharge
    {
        public double Gwei { get; set; }
     
        public TimeSpan WaitTime { get; set; }
    }
}
