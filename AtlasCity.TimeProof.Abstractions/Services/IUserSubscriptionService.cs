using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IUserSubscriptionService
    {
        Task<PaymentIntentResponse> GetPaymentIntent(string userId, string pricePlanId, CancellationToken cancellationToken);

        Task<SetupIntentResponse> CreateSetupIntent(string userId, CancellationToken cancellationToken);

        Task ProcessPayment(string userId, string paymentMethodId, string pricePlanId, CancellationToken cancellationToken);

        Task<PaymentMethodResponse> GetCustomerPaymentMethod(string userId, CancellationToken cancellationToken);

        Task UpdateCustomerPaymentMethod(string userId, string paymentMethodId, AddressDao newAddress, CancellationToken cancellationToken);

        Task ChangePricePlan(string userId, string pricePlanId, CancellationToken cancellationToken);

        Task CancelPendingPricePlan(string userId, string pendingPricePlanId, CancellationToken cancellationToken);
    }
}
