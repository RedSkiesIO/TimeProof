using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Messages;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampQueueService : ITimestampQueueService
    {
        private const string QueueName = "pendingtimestamps";
        private readonly CloudQueue messageQueue;
        private readonly ILogger _logger;

        public TimestampQueueService(string queueStorageConnectionString, ILogger logger)
        {
            Guard.Argument(queueStorageConnectionString, nameof(queueStorageConnectionString)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(logger, nameof(logger)).NotNull();

            var storageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            messageQueue = queueClient.GetQueueReference(QueueName);
            messageQueue.CreateIfNotExists();

            _logger = logger;
        }

        public async Task AddTimestampMessage(TimestampQueueMessage message, CancellationToken cancellationToken)
        {
            Guard.Argument(message, nameof(message)).NotNull();

            var jsonMessage = JsonConvert.SerializeObject(message);

            var queueMessage = new CloudQueueMessage(jsonMessage);
            await messageQueue.AddMessageAsync(queueMessage, cancellationToken);
        }

        public async Task<IEnumerable<TimestampQueueMessage>> GetTimestampMessage(int mesageCount, TimeSpan visibilityTimeout, CancellationToken cancellationToken)
        {
            Guard.Argument(mesageCount, nameof(mesageCount)).InRange(0, 33);
            Guard.Argument(visibilityTimeout, nameof(visibilityTimeout)).InRange(TimeSpan.MinValue, new TimeSpan(1, 0, 0));

            var returnList =  new List<TimestampQueueMessage>();

            var jsonMessages = (await messageQueue.GetMessagesAsync(mesageCount, visibilityTimeout, null, null, cancellationToken)).ToList();
            if (jsonMessages.Any())
            {
                for( var messageIndex = 0; messageIndex < jsonMessages.Count(); messageIndex++)
                {
                    var queueItem = jsonMessages[messageIndex];
                    var deserializedMessage = JsonConvert.DeserializeObject<TimestampQueueMessage>(queueItem.AsString);
                    deserializedMessage.Id = queueItem.Id;
                    deserializedMessage.PopReceipt = queueItem.PopReceipt;

                    returnList.Add(deserializedMessage);
                }
            }

            return returnList;
        }

        public async Task RemoveMessage(string messageId, string popReceipt, CancellationToken cancellationToken)
        {
            Guard.Argument(messageId, nameof(messageId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(popReceipt, nameof(popReceipt)).NotNull().NotEmpty().NotWhiteSpace();

            await messageQueue.DeleteMessageAsync(messageId, popReceipt, cancellationToken);

            _logger.Information($"Successfully removed pending time stamp message from the queue with '{messageId}' message identifier. The pop receipt value is '{popReceipt}'");
        }
    }
}