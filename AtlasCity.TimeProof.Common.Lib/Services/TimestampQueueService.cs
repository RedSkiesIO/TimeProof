using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Messages;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampQueueService : ITimestampQueueService
    {
        private const string QueueName = "TimestampMessage";
        private readonly CloudQueue messageQueue;

        public TimestampQueueService(string queueStorageConnectionString)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(queueStorageConnectionString);

            var storageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            messageQueue = queueClient.GetQueueReference(QueueName);
            messageQueue.CreateIfNotExists();
        }

        public async Task AddTimestampMessage(TimestampQueueMessage message, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(message);

            var jsonMessage = JsonConvert.SerializeObject(message);

            var queueMessage = new CloudQueueMessage(jsonMessage);
            await messageQueue.AddMessageAsync(queueMessage, cancellationToken);
        }
    }
}
