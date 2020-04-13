using System;

namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentIntentDao
    {
        public string Id { get; set; }

        public string ClientSecret { get; set; }

        public long Amount { get; set; }

        public DateTime Created { get; set; }
    }
}