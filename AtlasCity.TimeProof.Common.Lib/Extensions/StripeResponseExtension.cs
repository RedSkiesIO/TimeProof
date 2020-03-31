using AtlasCity.TimeProof.Abstractions.DAO;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class StripeResponseExtension
    {
        public static PaymentResponseDao ToStripeResponseDao(this StripeResponse stripeResponse)
        {
            if (stripeResponse != null)
            {
                var stripeResponseDao = new PaymentResponseDao
                {
                    Content = stripeResponse.Content,
                    RequestId = stripeResponse.RequestId,
                };

                return stripeResponseDao;
            }

            return null;
        }
    }
}