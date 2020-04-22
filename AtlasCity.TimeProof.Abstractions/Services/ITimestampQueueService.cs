using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Messages;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface ITimestampQueueService
    {
        Task AddTimestampMessage(TimestampQueueMessage message, CancellationToken cancellationToken);

        Task<IEnumerable<TimestampQueueMessage>> GetTimestampMessage(int mesageCount, TimeSpan visibilityTimeout, CancellationToken cancellationToken);

        Task RemoveMessage(string messageId, string popReceipt, CancellationToken cancellationToken);
    }
}
