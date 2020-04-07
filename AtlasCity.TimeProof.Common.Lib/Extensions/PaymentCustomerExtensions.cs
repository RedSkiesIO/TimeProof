using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class PaymentCustomerExtensions
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

        public static PaymentMethodDao ToPaymentMethodDao(this PaymentMethod paymentMethod)
        {
            if (paymentMethod != null)
            {
                var paymentMethodDao = new PaymentMethodDao()
                {
                    Id = paymentMethod.Id,
                    PaymentCustomerId = paymentMethod.CustomerId,

                    Card = paymentMethod.Card.ToCardDao()
                };

                return paymentMethodDao;
            }

            return null;
        }

        public static CardDao ToCardDao(this PaymentMethodCard paymentMethodCard)
        {
            if (paymentMethodCard != null)
            {
                var cardDao = new CardDao()
                {

                    Last4 = paymentMethodCard.Last4,
                    Brand = paymentMethodCard.Brand,
                    ExpMonth = paymentMethodCard.ExpMonth,
                    ExpYear = paymentMethodCard.ExpYear,
                    Issuer = paymentMethodCard.Issuer,
                    Country = paymentMethodCard.Country
                };

                return cardDao;
            }

            return null;
        }
    }
}
