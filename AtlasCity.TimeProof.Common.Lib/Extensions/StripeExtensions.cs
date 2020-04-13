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
                Amount = paymentIntent.Amount.HasValue ? paymentIntent.Amount.Value : 0,
                Created = paymentIntent.Created,
            };
        }
    }
}
