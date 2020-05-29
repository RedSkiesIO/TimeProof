using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEthClient
    {
        Task<double> GetCryptoCurrencyValue(string crypoCurrencyCode, string currencyCode, CancellationToken cancellationToken);

        Task<string> GetJsonResponseContent(string clientUri, CancellationToken cancellationToken);
    }
}
