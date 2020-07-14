using AtlasCity.TimeProof.Abstractions.DAO;
using System;

namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentIntentDao
    {
        public string Id { get; set; }

        public string ClientSecret { get; set; }

        public long Amount { get; set; }

        public DateTime Created { get; set; }

        public string CarIssuer { get; set; }

        public string Last4 { get; set; }

        public AddressDao Address { get; set; }
    }
}