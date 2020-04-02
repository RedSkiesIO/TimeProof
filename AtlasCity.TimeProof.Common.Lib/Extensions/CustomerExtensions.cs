using AtlasCity.TimeProof.Abstractions.DAO;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class CustomerExtensions
    {
        public static PaymentCustomerDao ToPaymentCustomerDao(this Customer customer)
        {
            if(customer != null)
            {
                var user = new PaymentCustomerDao()
                {
                    Email = customer.Email,
                    Id = customer.Id
                };

                user.Address = customer.Address.ToAddressDao();

                return user;
            }

            return null;
        }

        public static AddressDao ToAddressDao(this Address address)
        {
            if (address != null)
            {
                return new AddressDao()
                {
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    City = address.City,
                    State = address.State,
                    Postcode = address.PostalCode,
                    Country = address.Country,
                };
            }

            return null;
        }
    }
}
