using AtlasCity.TimeProof.Abstractions.Retry;
using Dawn;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Retry
{
    public class TimescribeRetryOnException: ITimescribeRetryOnException
    {
        private readonly int _retryCount;
        private readonly int _minDelayInSeconds;
        private readonly int _maxDelayInSeconds;
        private readonly Random random = new Random();

        public TimescribeRetryOnException(int retryCount, int minDelayInSeconds, int maxDelayInSeconds)
        {
            Guard.Argument(retryCount, nameof(retryCount)).InRange(1, int.MaxValue);
            Guard.Argument(minDelayInSeconds, nameof(minDelayInSeconds)).InRange(1, int.MaxValue);
            Guard.Argument(maxDelayInSeconds, nameof(maxDelayInSeconds)).InRange(1, int.MaxValue);
            Guard.Argument(minDelayInSeconds, nameof(minDelayInSeconds)).InRange(minDelayInSeconds, maxDelayInSeconds);

            _retryCount = retryCount;
            _minDelayInSeconds = minDelayInSeconds;
            _maxDelayInSeconds = maxDelayInSeconds;
        }

        public async Task Retry(List<Type> allowedExceptions, Action operation, ILogger logger)
        {
            Guard.Argument(allowedExceptions, nameof(allowedExceptions)).NotNull();
            Guard.Argument(operation, nameof(operation)).NotNull();
            Guard.Argument(logger, nameof(logger)).NotNull();

            var attempts = 0;
            do
            {
                bool canRetry = true;
                try
                {
                    attempts++;
                    operation();

                    break;
                }
                catch (Exception ex)
                {
                    if (allowedExceptions.Any() && !allowedExceptions.Any(s => s.Equals(ex.GetType())))
                    {
                        canRetry = false;
                    }

                    if (!canRetry || attempts == _retryCount)
                    {
                        throw;
                    }

                    logger.Error($"Exception caught on attempt {attempts} - will retry again.", ex);

                    await Task.Delay(new TimeSpan(0, 0, 0, random.Next(_minDelayInSeconds, _maxDelayInSeconds)));
                }
            } while (true);
        }

        public async Task RetryAsync(List<Type> allowedExceptions, Func<Task> operation, ILogger logger, CancellationToken cancellationToken)
        {
            Guard.Argument(allowedExceptions, nameof(allowedExceptions)).NotNull();
            Guard.Argument(operation, nameof(operation)).NotNull();
            Guard.Argument(logger, nameof(logger)).NotNull();

            var attempts = 0;
            do
            {
                var canRetry = true;
                try
                {
                    attempts++;
                    await operation();

                    break;
                }
                catch (Exception ex)
                {
                    if (allowedExceptions.Any() && allowedExceptions.All(s => s != ex.GetType()))
                    {
                        canRetry = false;
                    }

                    if (!canRetry || attempts == _retryCount)
                    {
                        throw;
                    }

                    logger.Error($"Exception caught on attempt {attempts} - will retry again.", ex);

                    await Task.Delay(new TimeSpan(0, 0, 0, random.Next(_minDelayInSeconds, _maxDelayInSeconds)));
                }
            } while (true || !cancellationToken.IsCancellationRequested);
        }
    }
}
