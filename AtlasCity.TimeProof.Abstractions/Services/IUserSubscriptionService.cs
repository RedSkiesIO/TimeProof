using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IUserSubscriptionService
    {
        Task<PaymentIntentDao> GetPaymentIntent(string userId, string pricePlanId, CancellationToken cancellationToken);

        Task<SetupIntentDao> CreateSetupIntent(string userId, CancellationToken cancellationToken);

        Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken);

        Task<PaymentMethodDao> GetCustomerPaymentMethod(string userId, CancellationToken cancellationToken);
    }
}
