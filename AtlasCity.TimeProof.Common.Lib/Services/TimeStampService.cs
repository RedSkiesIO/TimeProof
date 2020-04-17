using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using Serilog;
using AtlasCity.TimeProof.Common.Lib.Exceptions;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampService : ITimestampService
    {
        private readonly ILogger _logger;
        private readonly ITimestampRepository _timestampRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISignatureHelper _signatureHelper;

        public TimestampService(
           ILogger logger,
           ITimestampRepository timestampRepository,
           IUserRepository userRepository, ISignatureHelper signatureHelper)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(timestampRepository);
            AtlasGuard.IsNotNull(userRepository);
            AtlasGuard.IsNotNull(signatureHelper);

            _logger = logger;
            _timestampRepository = timestampRepository;
            _userRepository = userRepository;
            _signatureHelper = signatureHelper;
        }

        public async Task<IEnumerable<TimestampDao>> GetUesrTimestamps(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            //TODO: Sudhir Paging
            return await _timestampRepository.GetTimestampByUser(userId, cancellationToken);
        }

        public async Task<TimestampDao> GenerateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(timestamp.UserId);
            AtlasGuard.IsNotNull(timestamp);

            var user = await _userRepository.GetUserById(timestamp.UserId, cancellationToken);
            if (user == null)
            {
                var message = $"Unable to find the user with user identifier '{timestamp.UserId}'.";
                _logger.Error(message);
                throw new UserException(message);
            }

            if (user.RemainingTimeStamps < 1)
            {
                var message = $"Not sufficient stamps left for the user '{user.Id}' with price plan '{user.CurrentPricePlanId}'.";
                _logger.Error(message);
                throw new TimestampException(message);
            }

            bool proofVerified = _signatureHelper.VerifyStamp(timestamp);
            if (!proofVerified)
            {
                var message = $"Unable to verify the signature '{timestamp.Signature}'.";
                _logger.Warning(message);
                throw new TimestampException(message);
            }

            await SendTransaction(timestamp);
            var newTimestamp = await _timestampRepository.CreateTimestamp(timestamp, cancellationToken);

            user.RemainingTimeStamps--;
            await _userRepository.UpdateUser(user, cancellationToken);

            _logger.Information($"Successfully created timestamp for user '{user.Id}' with transaction '{newTimestamp.TransactionId}'");

            return newTimestamp;
        }

        public async Task<TimestampDao> UpdateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(timestamp.UserId);
            AtlasGuard.IsNotNull(timestamp);

            var user = await _userRepository.GetUserById(timestamp.UserId, cancellationToken);
            if (user == null)
            {
                var message = $"Unable to find the user with user identifier '{timestamp.UserId}'.";
                _logger.Error(message);
                throw new UserException(message);
            }

            var updatedTimestamp = await _timestampRepository.UpdateTimestamp(timestamp, cancellationToken);

            _logger.Information($"Successfully updated timestamp for user '{user.Id}' with transaction '{updatedTimestamp.TransactionId}'");

            return updatedTimestamp;
        }

        private async Task<TimestampDao> SendTransaction(TimestampDao timestamp)
        {
            ManagedAccount account = new ManagedAccount(Constants.ACCOUNT_ADDRESS, "");

            var web3 = new Web3(account, Constants.NODE_ENDPOINT);

            var user = "";//(jwtDecode(req.headers.authorization)).sub;

            string proofStr = JsonConvert.SerializeObject(
                new
                {
                    file = timestamp.FileName,
                    hash = timestamp.Hash,
                    publicKey = timestamp.PublicKey,
                    signature = timestamp.Signature
                });

            var txData = HexStringUTF8ConvertorExtensions.ToHexUTF8(proofStr);

            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreatePending());

            var res = (
                privateKey: Constants.SECRET_KEY,
                chainId: Nethereum.Signer.Chain.Kovan,
                to: Constants.TO_ADDRESS,
                amount: Web3.Convert.ToWei(0, UnitConversion.EthUnit.Gwei),
                nonce: currentNonce,
                gasPrice: Web3.Convert.ToWei(Constants.GAS_PRICE, UnitConversion.EthUnit.Gwei),
                gasLimit: new BigInteger(100000),
                data: txData
            );

            var encoded = Web3.OfflineTransactionSigner.SignTransaction(res.privateKey,
                res.chainId, res.to, res.amount, res.nonce, res.gasPrice, res.gasLimit, res.data);

            var verified = Web3.OfflineTransactionSigner.VerifyTransaction(encoded);

            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            }

            timestamp.BlockNumber = -1;
            timestamp.TransactionId = txId;
            timestamp.Nonce = (long)currentNonce.Value;
            timestamp.Network = Nethereum.Signer.Chain.Kovan.ToString();

            return timestamp;
        }
    }
}