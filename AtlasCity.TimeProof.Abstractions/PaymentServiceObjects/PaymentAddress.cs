namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentAddress
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }
    }
}