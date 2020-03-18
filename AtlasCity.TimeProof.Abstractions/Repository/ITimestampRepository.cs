using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface ITimestampRepository
    {
        public Task<IEnumerable<TimestampDao>> GetTimestampByUser(string userId, CancellationToken cancellationToken);

        public Task<TimestampDao> CreateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken);

        public Task<TimestampDao> UpdateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken);
    }
}
