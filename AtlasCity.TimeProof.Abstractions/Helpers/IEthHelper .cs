using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.EthResponse;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEthHelper
    {
        bool VerifyStamp(TimestampDao timestamp);

        EthSettings GetEthSettings();

        Task<int> GetGasPrice(double amountInPence, CancellationToken cancellationToken);

        Task<EthGasStationPrice> GetGasStationPrice(string apiEndPoint, CancellationToken cancellationToken);
    }
}
