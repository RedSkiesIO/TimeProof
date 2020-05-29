using AtlasCity.TimeProof.Abstractions.Enums;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using Microsoft.Azure.WebJobs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.AzureFunction
{
    public class TimestampProcessingTrigger
    {
        private readonly ILogger _logger;
        private readonly ITimestampQueueService _timestampQueueService;
        private readonly ITimestampRepository _timestampRepository;
        private readonly VeryfyTransactionSettings _settings;

        public TimestampProcessingTrigger(
            ILogger logger,
            ITimestampQueueService timestampQueueService,
            ITimestampRepository timestampRepository,
            VeryfyTransactionSettings settings)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampQueueService, nameof(timestampQueueService)).NotNull();
            Guard.Argument(timestampRepository, nameof(timestampRepository)).NotNull();
            Guard.Argument(settings, nameof(settings)).NotNull();

            _logger = logger;
            _timestampQueueService = timestampQueueService;
            _timestampRepository = timestampRepository;
            _settings = settings;
        }

        [FunctionName("TimestampProcessingFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer)
        {
            var cancellationToken = new CancellationToken();
            var pendingTimestamps = await _timestampQueueService.GetTimestampMessage(5, new TimeSpan(0, 1, 0), cancellationToken);
            if (!pendingTimestamps.Any())
            {
                _logger.Information($"Unable to find any message at '{DateTime.UtcNow}'.");
                return;
            }

            _logger.Information($"Retrieved '{pendingTimestamps.Count()}' messages at '{DateTime.UtcNow}'.");

            foreach (var pendingTimestamp in pendingTimestamps)
            {
                var hasTransactionMinned = await HasTransactionMinned(pendingTimestamp.TimestampId, pendingTimestamp.TransactionId, pendingTimestamp.IsPremiumPlan, cancellationToken);
                if (hasTransactionMinned)
                {
                    _logger.Information($"Removing message with transactionId '{pendingTimestamp.TransactionId}' and timestampId '{pendingTimestamp.TransactionId}' for message Id '{pendingTimestamp.Id}'.");
                    await _timestampQueueService.RemoveMessage(pendingTimestamp.Id, pendingTimestamp.PopReceipt, cancellationToken);
                }
            }
        }

        private async Task<bool> HasTransactionMinned(string timestampId, string transactionId, bool isPremiumPlan, CancellationToken cancellationToken)
        {
            Guard.Argument(timestampId, nameof(timestampId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(transactionId, nameof(transactionId)).NotNull().NotEmpty().NotWhiteSpace();

            IWeb3 web3 = null;
            if (isPremiumPlan)
            {
                var premiumAccount = new ManagedAccount(_settings.NetheriumPremiumAccountFromAddress, string.Empty);
                web3 = new Web3(premiumAccount, _settings.NetheriumAccountNodeEndpoint);
            }
            else
            {
                var basicAccount = new ManagedAccount(_settings.NetheriumBasicAccountFromAddress, string.Empty);
                web3 = new Web3(basicAccount, _settings.NetheriumAccountNodeEndpoint);
            }

            _logger.Information($"Checking ETH transaction with transactionId '{transactionId}' for timestampId '{timestampId}' on '{web3.TransactionManager.Account.Address}'.");
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId);


            if (receipt == null || receipt.BlockNumber.Value < 0)
            {
                _logger.Information($"ETH transaction with transactionId '{transactionId}' still pending.");
                return false;
            }

            _logger.Information($"ETH transaction with transactionId '{transactionId}' succeeded.");
            var timestamp = await _timestampRepository.GetTimestampById(timestampId, cancellationToken);
            if (timestamp.Status != TimestampState.Pending)
            {
                _logger.Warning($"It is not ideal work-flow. Investigate with time stamp identifier '{timestamp.Id}' and transaction identifier '{timestamp.TransactionId}'");
            }
            else
            {
                timestamp.BlockNumber = (long)receipt.BlockNumber.Value;

                timestamp.Status = TimestampState.Succeeded;
                _logger.Information($"Transaction successful for an user '{timestamp.UserId}' with transaction identifier '{timestamp.TransactionId}'.");
                await _timestampRepository.UpdateTimestamp(timestamp, cancellationToken);
            }

            return true;
        }
    }
}