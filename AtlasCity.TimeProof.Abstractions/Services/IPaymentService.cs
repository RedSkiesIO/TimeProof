using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<string> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken);

        Task<string> CreatePaymentCustomer(UserDao customer, CancellationToken cancellationToken);

        Task<UserDao> GetCustomerById(string paymentCustomerId, CancellationToken cancellationToken);

        Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken);

        Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken);
    }
}
