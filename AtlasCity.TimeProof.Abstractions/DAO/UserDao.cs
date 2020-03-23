using Dawn;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class UserDao
    {
        public UserDao(string email)
        {
            Guard.Argument(email, nameof(email)).NotWhiteSpace("email is missing for an user.");
            Email = email;
        }

        public string Email { get; private set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PaymentCustomerId { get; set; }

        public AddressDao Address { get; set; }
    }
}
