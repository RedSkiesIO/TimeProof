using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IPaymentRepository
    {
        Task<PaymentIntentDao> CreatePaymentReceived(PaymentIntentDao paymentIntent, CancellationToken cancellationToken);
    }
}
