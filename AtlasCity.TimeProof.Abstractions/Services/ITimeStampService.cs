using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface ITimestampService
    {
        public Task<IEnumerable<TimestampDao>> GetUesrTimestamps(string userId, CancellationToken cancellationToken);

        public Task<TimestampDao> GenerateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken);

        public Task<TimestampDao> UpdateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken);
    }
}
