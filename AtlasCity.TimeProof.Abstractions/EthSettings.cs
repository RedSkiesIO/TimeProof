namespace AtlasCity.TimeProof.Abstractions
{
    public class EthSettings
    {
        public string BasicAccountSecretKey { get; set; }
        public string PremiumAccountSecretKey { get; set; }
        public string ToAddress { get; set; }
        public string Network { get; set; }
        public string GasStationAPIEndpoint { get; set; }
    }
}
