using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Retry
{
    public interface ITimescribeRetryOnException
    {
        Task Retry(List<Type> allowedExceptions, Action operation, ILogger logger);
        Task RetryAsync(List<Type> allowedExceptions, Func<Task> operation, ILogger logger, CancellationToken cancellationToken);
    }
}
