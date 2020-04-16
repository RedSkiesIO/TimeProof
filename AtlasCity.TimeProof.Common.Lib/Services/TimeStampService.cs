using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampService : ITimestampService
    {
        private readonly ITimestampRepository _timestampRepository;

        public TimestampService(ITimestampRepository timeStampRepository)
        {
            AtlasGuard.IsNotNull(timeStampRepository);

            _timestampRepository = timeStampRepository;
        }

        public async Task<IEnumerable<TimestampDao>> GetUesrTimestamps(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            //TODO: Sudhir Paging
            return await _timestampRepository.GetTimestampByUser(userId, cancellationToken);
        }

        public async Task<TimestampDao> GenerateTimestamp(string userId, TimestampDao timestamp, CancellationToken cancellationToken)
        { 
            AtlasGuard.IsNotNullOrWhiteSpace(timestamp.UserId);
            AtlasGuard.IsNotNull(timestamp);

            // TODO: Sudhir Check if user have stamp remaining

            return await _timestampRepository.CreateTimestamp(userId, timestamp, cancellationToken);

            // TODO: Sudhir Decrease the remaining timestamp
        }
    }
}