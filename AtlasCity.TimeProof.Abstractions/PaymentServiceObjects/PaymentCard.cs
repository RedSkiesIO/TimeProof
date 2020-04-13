namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentCard
    {
        public string Last4 { get; set; }
        
        public string Brand { get; set; }
        
        public long ExpMonth { get; set; }
     
        public long ExpYear { get; set; }

        public string Issuer { get; set; }

        public string Country { get; set; }
    }
}
