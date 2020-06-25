using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.EthResponse;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EthHelper : IEthHelper
    {
        const int Total_Gwei_In_Eth = 1000000000;
        const int Gas_Limit = 30000;
        const int Default_Gas_Price = 2;

        private readonly EthSettings _ethSettings;
        private readonly IEthClient _ethClient;
        private readonly ILogger _logger;

        public EthHelper(EthSettings ethSettings, IEthClient ethClient, ILogger logger)
        {
            Guard.Argument(ethSettings, nameof(ethSettings)).NotNull();
            Guard.Argument(ethClient, nameof(ethClient)).NotNull();
            Guard.Argument(logger, nameof(logger)).NotNull();

            _ethSettings = ethSettings;
            _ethClient = ethClient;
            _logger = logger;
        }

        public bool VerifyStamp(TimestampDao timestamp)
        {
            var fileHash = timestamp.FileHash.ToUpper().FromBase32();
            var signature = timestamp.Signature.ToUpper().FromBase32();
            var publicKey = new Ed25519PublicKeyParameters(timestamp.PublicKey.ToUpper().FromBase32(), 0);

            var verifier = new Ed25519Signer();
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(fileHash, 0, fileHash.Length);
            return verifier.VerifySignature(signature);
        }

        public EthSettings GetEthSettings()
        {
            return _ethSettings;
        }

        public async Task<int> GetGasPrice(double amountInPence, CancellationToken cancellationToken)
        {
            Guard.Argument(amountInPence, nameof(amountInPence)).InRange(1, 1000);

            var oneEthToGBP = await _ethClient.GetCryptoCurrencyValue("ETH", "GBP", cancellationToken);
            if (amountInPence <= 0)
                return Default_Gas_Price;

            var totalGweiInOnePence = Total_Gwei_In_Eth / (oneEthToGBP * 100);

            var gweiToSpend = totalGweiInOnePence * amountInPence;

            var gasPrice = Convert.ToInt32(Math.Floor(gweiToSpend / Gas_Limit));

            return gasPrice;
        }

        public async Task<EthCharge> GetFreePlanGwei(string apiEndPoint, CancellationToken cancellationToken)
        {
            Guard.Argument(apiEndPoint, nameof(apiEndPoint)).NotNull().NotEmpty().NotWhiteSpace();

            var responseContent = await _ethClient.GetJsonResponseContent(apiEndPoint, cancellationToken);
            var ethGasStationPrice = JsonConvert.DeserializeObject<EthGasStationPrice>(responseContent);

            if (ethGasStationPrice != null)
            {
                if (ethGasStationPrice.SafeLowGwei < ethGasStationPrice.AverageGwei / 2)
                {
                    _logger.Information($"FastGwei: '{ethGasStationPrice.FastGwei}', AverageGwei: '{ethGasStationPrice.AverageGwei}', SafeLowGwei: '{ethGasStationPrice.SafeLowGwei}'");
                    return new EthCharge { Gwei = ethGasStationPrice.AverageGwei, WaitTime = ethGasStationPrice.AverageWaitTime };
                }
                else
                {
                    return new EthCharge { Gwei = ethGasStationPrice.SafeLowGwei, WaitTime = ethGasStationPrice.SafeLowWaitTime };
                }
            }

            return null;
        }

        public async Task<EthCharge> GetPaidPlanGwei(string apiEndPoint, CancellationToken cancellationToken)
        {
            Guard.Argument(apiEndPoint, nameof(apiEndPoint)).NotNull().NotEmpty().NotWhiteSpace();

            var responseContent = await _ethClient.GetJsonResponseContent(apiEndPoint, cancellationToken);
            var ethGasStationPrice = JsonConvert.DeserializeObject<EthGasStationPrice>(responseContent);

            if (ethGasStationPrice != null)
                return new EthCharge { Gwei = ethGasStationPrice.FastGwei, WaitTime = ethGasStationPrice.FastWaitTime };

            return null;
        }
    }
}