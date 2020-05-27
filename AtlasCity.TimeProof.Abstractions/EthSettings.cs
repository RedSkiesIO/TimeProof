namespace AtlasCity.TimeProof.Abstractions
{
    public class EthSettings
    {
        public string SecretKey { get; set; }
        public string ToAddress { get; set; }
        public string Network { get; set; }
        public string GasStationAPIEndpoint { get; set; }
    }
}
