using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Enums;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using Microsoft.Azure.WebJobs;
using Nethereum.Web3;
using Serilog;

namespace AtlasCity.TimeProof.AzureFunction
{
    public class TimestampProcessingTrigger
    {
        private readonly ILogger _logger;
        private readonly ITimestampQueueService _timestampQueueService;
        private readonly ITimestampRepository _timestampRepository;
        private readonly IWeb3 _netheriumWeb3;

        public TimestampProcessingTrigger(
            ILogger logger,
            ITimestampQueueService timestampQueueService,
            ITimestampRepository timestampRepository,
            IWeb3 netheriumWeb3)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampQueueService, nameof(timestampQueueService)).NotNull();
            Guard.Argument(timestampRepository, nameof(timestampRepository)).NotNull();
            Guard.Argument(netheriumWeb3, nameof(netheriumWeb3)).NotNull();

            _logger = logger;
            _timestampQueueService = timestampQueueService;
            _timestampRepository = timestampRepository;
            _netheriumWeb3 = netheriumWeb3;
        }

        //[FunctionName("TimestampProcessingFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer)
        {
            return;
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
                var hasTransactionMinned = await HasTransactionMinned(pendingTimestamp.TimestampId, pendingTimestamp.TransactionId, cancellationToken);
                if (hasTransactionMinned)
                {
                    _logger.Information($"Removing message with transactionId '{pendingTimestamp.TransactionId}' and timestampId '{pendingTimestamp.TransactionId}' for message Id '{pendingTimestamp.Id}'.");
                    await _timestampQueueService.RemoveMessage(pendingTimestamp.Id, pendingTimestamp.PopReceipt, cancellationToken);
                }
            }
        }

        private async Task<bool> HasTransactionMinned(string timestampId, string transactionId, CancellationToken cancellationToken)
        {
            Guard.Argument(timestampId, nameof(timestampId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(transactionId, nameof(transactionId)).NotNull().NotEmpty().NotWhiteSpace();

            _logger.Information($"Checking ETH transaction with transactionId '{transactionId}' for timestampId '{timestampId}'.");
            var receipt = await _netheriumWeb3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId);
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