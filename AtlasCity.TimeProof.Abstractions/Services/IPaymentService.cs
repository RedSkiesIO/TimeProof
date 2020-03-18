using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<string> TakePayment(PaymentDao payment);
    }
}
