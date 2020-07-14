using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentCustomerDao
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public AddressDao Address { get; set; }
    }
}