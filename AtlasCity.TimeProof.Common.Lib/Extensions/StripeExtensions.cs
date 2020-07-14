using System.Linq;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class StripeExtensions
    {
        public static SetupIntentDao ToSetupIntentDao(this SetupIntent setupIntent)
        {
            if (setupIntent == null)
                return null;

            return new SetupIntentDao
            {
                Id = setupIntent.Id,
                CustomerId = setupIntent.CustomerId,
                ClientSecret = setupIntent.ClientSecret,
            };
        }

        public static PaymentIntentDao ToPaymentIntentDao(this PaymentIntent paymentIntent)
        {
            if (paymentIntent == null)
                return null;

            return new PaymentIntentDao
            {
                Id = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = paymentIntent.Amount ?? 0,
                Created = paymentIntent.Created,

                CarIssuer = paymentIntent.Charges?.FirstOrDefault()?.PaymentMethodDetails?.Card?.Brand,
                Last4 = paymentIntent.Charges?.FirstOrDefault()?.PaymentMethodDetails?.Card?.Last4,

                Address = paymentIntent.Charges?.FirstOrDefault()?.BillingDetails?.Address.ToAddress()
            };
        }
    }
}
