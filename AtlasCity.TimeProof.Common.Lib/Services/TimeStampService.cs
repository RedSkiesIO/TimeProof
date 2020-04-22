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
using Nethereum.RPC.Eth.DTOs;
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
        private readonly ISignatureHelper _signatureHelper;
        private readonly ITimestampQueueService _timestampQueueService;
        private readonly IWeb3 _netheriumWeb3;

        public TimestampService(
           ILogger logger,
           ITimestampRepository timestampRepository,
           IUserRepository userRepository, 
           ISignatureHelper signatureHelper,
           ITimestampQueueService timestampQueueService,
           IWeb3 netheriumWeb3)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampRepository, nameof(timestampRepository)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(signatureHelper, nameof(signatureHelper)).NotNull();
            Guard.Argument(timestampQueueService, nameof(timestampQueueService)).NotNull();
            Guard.Argument(netheriumWeb3, nameof(netheriumWeb3)).NotNull();

            _logger = logger;
            _timestampRepository = timestampRepository;
            _userRepository = userRepository;
            _signatureHelper = signatureHelper;
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
        }

        public async Task<TimestampDao> GetTimestampDetails(string timestampId, CancellationToken cancellationToken)
        {
            Guard.Argument(timestampId, nameof(timestampId)).NotNull().NotEmpty().NotWhiteSpace();

            var timestamp = await _timestampRepository.GetTimestampById(timestampId, cancellationToken);
            return timestamp;
        }

        private async Task<TimestampDao> SendTransaction(TimestampDao timestamp)
        {
            bool proofVerified = _signatureHelper.VerifyStamp(timestamp);
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

            // TODO: Sudhir DI
            const int GAS_PRICE = 3;
            const string SECRET_KEY = "35641a29e6da0ae19bc594c6611bb5660ce2a038f9c2c9918334e68101f314f6";
            const string TO_ADDRESS = "0x1bb31D596c34bd81e1F0BE1edF3840a7b43dd9CD";

            var encoded = Web3.OfflineTransactionSigner.SignTransaction(
                    SECRET_KEY,
                    Nethereum.Signer.Chain.Kovan,
                    TO_ADDRESS,
                    Web3.Convert.ToWei(0, UnitConversion.EthUnit.Gwei),
                    currentNonce,
                    Web3.Convert.ToWei(GAS_PRICE, UnitConversion.EthUnit.Gwei),
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
            timestamp.Network = Nethereum.Signer.Chain.Kovan.ToString(); // TODO: It need to come from the configuration as in production it can differ
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