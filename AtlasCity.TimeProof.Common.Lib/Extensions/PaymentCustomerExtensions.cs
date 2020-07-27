using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class PaymentCustomerExtensions
    {
        public static PaymentCustomerDao ToPaymentCustomerDao(this Customer customer)
        {
            if (customer == null)
                return null;

            return new PaymentCustomerDao
            {
                Email = customer.Email,
                Id = customer.Id,
                Address = customer.Address.ToAddress()
            };

        }

        public static AddressDao ToAddress(this Address address)
        {
            if (address == null)
                return null;

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

        public static PaymentMethodDao ToPaymentMethodDao(this PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                return null;

            return new PaymentMethodDao()
            {
                Id = paymentMethod.Id,
                PaymentCustomerId = paymentMethod.CustomerId,

                Card = paymentMethod.Card.ToPaymentCard(),
                Address = paymentMethod.BillingDetails?.Address.ToAddress()
            };
        }

        public static PaymentCard ToPaymentCard(this PaymentMethodCard paymentMethodCard)
        {
            if (paymentMethodCard == null)
                return null;

            return new PaymentCard()
            {
                Last4 = paymentMethodCard.Last4,
                Brand = paymentMethodCard.Brand,
                ExpMonth = paymentMethodCard.ExpMonth,
                ExpYear = paymentMethodCard.ExpYear,
                Issuer = paymentMethodCard.Issuer,
                Country = paymentMethodCard.Country
            };
        }
    }
}