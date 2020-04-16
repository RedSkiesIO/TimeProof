using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IPaymentRepository
    {
        Task<ProcessedPaymentDao> CreatePaymentReceived(ProcessedPaymentDao payment, CancellationToken cancellationToken);

        Task<ProcessedPaymentDao> GetLastPayment(string userId, CancellationToken cancellationToken);
    }
}
