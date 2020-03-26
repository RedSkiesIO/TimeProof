using System.Collections.Generic;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class PaymentDao
    {
        public string PaymentCustomerId { get; set; }

        public long Amount { get; set; }

        public string Email{ get; set; }

        public string Currency = "gbp";

        public List<string> PaymentMethodTypes = new List<string> { "card" };
    }
}
