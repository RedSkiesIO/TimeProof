using AtlasCity.TimeProof.Abstractions.DAO.Payment;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class StripeExtensions
    {
        public static SetupIntentDao ToSetupIntentDao(this SetupIntent setupIntent)
        {
            if (setupIntent != null)
            {
                var setupIntentDao = new SetupIntentDao
                {
                    Id = setupIntent.Id,
                    CustomerId = setupIntent.CustomerId,
                    ClientSecret = setupIntent.ClientSecret
                };

                return setupIntentDao;
            }

            return null;
        }

        public static PaymentIntentDao ToPaymentIntentDao(this PaymentIntent paymentIntent)
        {
            if (paymentIntent != null)
            {
                var paymentIntentDao = new PaymentIntentDao
                {
                    Id = paymentIntent.Id,
                    CustomerId = paymentIntent.CustomerId,
                    ClientSecret = paymentIntent.ClientSecret

                };

                return paymentIntentDao;
            }

            return null;
        }
    }
}
