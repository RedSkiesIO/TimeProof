using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IUserSubscriptionService
    {
        Task<SetupIntentDao> CreateSetupIntent(string userId, CancellationToken cancellationToken);

        Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken);
    }
}
