using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IPaymentRepository
    {
        Task<PaymentResponseDao> CreatePaymentReceived(PaymentResponseDao paymentResponse, CancellationToken cancellationToken);
    }
}
