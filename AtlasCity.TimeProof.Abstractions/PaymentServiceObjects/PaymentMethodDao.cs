using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentMethodDao
    {
        public string Id { get; set; }

        public string PaymentCustomerId { get; set; }

        public PaymentCard Card { get; set; }

        public AddressDao Address { get; set; }
    }
}