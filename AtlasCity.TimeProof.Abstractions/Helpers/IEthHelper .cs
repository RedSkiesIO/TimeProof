using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEthHelper
    {
        bool VerifyStamp(TimestampDao timestamp);

        EthSettings GetEthSettings();

        Task<int> GetGasPrice(double amountInPence, CancellationToken cancellationToken);
    }
}
