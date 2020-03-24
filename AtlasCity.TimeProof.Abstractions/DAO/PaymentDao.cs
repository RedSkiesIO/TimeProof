using System.Collections.Generic;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PaymentDao
    {
        public PaymentDao()
        {
            Currency = "gbp";
            PaymentMethodTypes = new List<string> { "card" };
        }

        public string PaymentCustomerId { get; set; }

        public long Amount { get; set; }

        public string Email{ get; set; }

        public string Currency { get; private set; }

        public List<string> PaymentMethodTypes { get; private set; }
    }
}
