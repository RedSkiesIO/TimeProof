using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentCustomer(UserDao customer, CancellationToken cancellationToken);

        Task<PaymentCustomerDao> GetCustomerById(string paymentCustomerId, CancellationToken cancellationToken);

        Task<PaymentMethodDao> GetCustomerPaymentMethod(string paymentCustomerId, CancellationToken cancellationToken);

        Task UpdateCustomerPaymentMethod(string paymentMethodId, AddressDao newAddress, CancellationToken cancellationToken);

        Task DeletePaymentCustomer(string paymentCustomerId, CancellationToken cancellationToken);

        Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken);

        Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken);

        Task<PaymentIntentDao> GetPaymentIntent(string paymentIntentId, CancellationToken cancellationToken);

        Task<PaymentIntentDao> GetPaymentIntentByCustomerId(string paymentCustomerId, CancellationToken cancellationToken);

        Task<PaymentIntentDao> CreatePaymentIntent(string paymentCustomerId, long amount, CancellationToken cancellationToken);

        Task<PaymentIntentDao> CollectPayment(string paymentCustomerId, long amount, CancellationToken cancellationToken);
    }
}
