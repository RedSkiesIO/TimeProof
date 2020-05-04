using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Enums;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Messages;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampService : ITimestampService
    {
        private readonly ILogger _logger;
        private readonly ITimestampRepository _timestampRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEthHelper _ethHelper;
        private readonly ITimestampQueueService _timestampQueueService;
        private readonly IWeb3 _netheriumWeb3;

        public TimestampService(
           ILogger logger,
           ITimestampRepository timestampRepository,
           IUserRepository userRepository,
           IEthHelper ethHelper,
           ITimestampQueueService timestampQueueService,
           IWeb3 netheriumWeb3)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampRepository, nameof(timestampRepository)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(ethHelper, nameof(ethHelper)).NotNull();
            Guard.Argument(timestampQueueService, nameof(timestampQueueService)).NotNull();
            Guard.Argument(netheriumWeb3, nameof(netheriumWeb3)).NotNull();

            _logger = logger;
            _timestampRepository = timestampRepository;
            _userRepository = userRepository;
            _ethHelper = ethHelper;
            _timestampQueueService = timestampQueueService;
            _netheriumWeb3 = netheriumWeb3;
        }

        public async Task<IEnumerable<TimestampDao>> GetUesrTimestamps(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            //TODO: Sudhir Paging
            return await _timestampRepository.GetTimestampByUser(userId, cancellationToken);
        }

        public async Task<TimestampDao> GenerateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            Guard.Argument(timestamp, nameof(timestamp)).NotNull();
            Guard.Argument(timestamp.UserId, nameof(timestamp.UserId)).NotNull().NotEmpty().NotWhiteSpace();

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

            try
            {
                await SendTransaction(timestamp);

                var newTimestamp = await _timestampRepository.CreateTimestamp(timestamp, cancellationToken);

                await _timestampQueueService.AddTimestampMessage(new TimestampQueueMessage { TimestampId = newTimestamp.Id, TransactionId = newTimestamp.TransactionId, Created = DateTime.UtcNow }, cancellationToken);

                user.RemainingTimeStamps--;
                await _userRepository.UpdateUser(user, cancellationToken);

                _logger.Information($"Successfully created time stamp for user '{user.Id}' with transaction '{newTimestamp.TransactionId}'");

                return newTimestamp;
            }
            catch (TimestampException ex)
            {
                _logger.Error(ex.Message);
                throw ex;
            }
            catch(RpcClientUnknownException ex)
            {
                _logger.Error(ex.Message);
                throw new RpcClientException(ex.Message);
            }
        }

        public async Task<TimestampDao> GetTimestampDetails(string timestampId, string requestedUserId, CancellationToken cancellationToken)
        {
            Guard.Argument(timestampId, nameof(timestampId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(requestedUserId, nameof(requestedUserId)).NotNull().NotEmpty().NotWhiteSpace();

            var timestamp = await _timestampRepository.GetTimestampById(timestampId, cancellationToken);
            if (timestamp != null && !timestamp.UserId.Equals(requestedUserId, StringComparison.InvariantCultureIgnoreCase))
            {
                var message = $"Invalid time stamp detail request. Detail requested by '{requestedUserId}' which belong to '{timestamp.UserId}' for identifier '{timestamp.Id}'";
                _logger.Warning(message);
                throw new TimestampException(message);
            }

            return timestamp;
        }

        private async Task<TimestampDao> SendTransaction(TimestampDao timestamp)
        {
            bool proofVerified = _ethHelper.VerifyStamp(timestamp);
            if (!proofVerified)
            {
                var message = $"Unable to verify the signature '{timestamp.Signature}'.";
                _logger.Warning(message);
                throw new TimestampException(message);
            }

            string proofStr = JsonConvert.SerializeObject(
                new
                {
                    file = timestamp.FileName,
                    hash = timestamp.FileHash,
                    publicKey = timestamp.PublicKey,
                    signature = timestamp.Signature
                });

            var txData = HexStringUTF8ConvertorExtensions.ToHexUTF8(proofStr);

            var currentNonce = await _netheriumWeb3.Eth.Transactions.GetTransactionCount.SendRequestAsync(_netheriumWeb3.TransactionManager.Account.Address, BlockParameter.CreatePending());

            var ethSettings = _ethHelper.GetEthSettings();
            if (!Enum.TryParse(ethSettings.Network, true, out Chain networkChain))
            {
                networkChain = Chain.MainNet;
                _logger.Warning($"Unable to parse '{ethSettings.Network}' to type '{typeof(Chain)}', so setting default to '{networkChain}'.");
            }

            var encoded = Web3.OfflineTransactionSigner.SignTransaction(
                    ethSettings.SecretKey,
                    Nethereum.Signer.Chain.Kovan,
                    ethSettings.ToAddress,
                    Web3.Convert.ToWei(0, UnitConversion.EthUnit.Gwei),
                    currentNonce,
                    Web3.Convert.ToWei(ethSettings.GasPrice, UnitConversion.EthUnit.Gwei),
                    new BigInteger(100000),
                    txData);

            // TODO: Veysel Ask if we are charged here
            var verified = Web3.OfflineTransactionSigner.VerifyTransaction(encoded);
            if (!verified)
            {
                var message = $"Unable to verify the transaction for data '{txData}'.";
                _logger.Error(message);
                throw new TimestampException(message);
            }

            var txId = await _netheriumWeb3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            timestamp.Nonce = (long)currentNonce.Value;
            timestamp.TransactionId = txId;
            timestamp.Network = networkChain.ToString();
            timestamp.BlockNumber = -1;

            // TODO: Veysel Ask if we are charged here
            if (string.IsNullOrWhiteSpace(txId))
            {
                timestamp.Status = TimestampState.Failed;
                var message = $"Transaction failed for an user '{timestamp.UserId}' with file name '{timestamp.FileName}'.";
                _logger.Error(message);
            }

            return timestamp;
        }
    }
}