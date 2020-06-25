using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.EthResponse;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEthHelper
    {
        bool VerifyStamp(TimestampDao timestamp);

        EthSettings GetEthSettings();

        Task<int> GetGasPrice(double amountInPence, CancellationToken cancellationToken);

        Task<EthCharge> GetFreePlanGwei(string apiEndPoint, CancellationToken cancellationToken);

        Task<EthCharge> GetPaidPlanGwei(string apiEndPoint, CancellationToken cancellationToken);
    }
}
