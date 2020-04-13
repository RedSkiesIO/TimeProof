namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class SetupIntentDao
    {
        public string Id { get; set; }
     
        public string CustomerId { get; set; }

        public string ClientSecret { get; set; }
    }
}
