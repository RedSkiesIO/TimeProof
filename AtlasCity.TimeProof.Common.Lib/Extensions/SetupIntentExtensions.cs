using AtlasCity.TimeProof.Abstractions.DAO;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class SetupIntentExtensions
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
    }
}
