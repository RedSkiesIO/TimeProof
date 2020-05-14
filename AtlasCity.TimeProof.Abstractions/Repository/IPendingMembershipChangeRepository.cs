using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface  IPendingMembershipChangeRepository
    {
        Task<PendingMembershipChangeDao> Add(PendingMembershipChangeDao membershipRenew, CancellationToken cancellationToken);

        Task<IEnumerable<PendingMembershipChangeDao>> GetByDate(DateTime renewDate, CancellationToken cancellationToken);

        Task<PendingMembershipChangeDao> GetByUser(string userId, CancellationToken cancellationToken);

        Task Delete(string id, CancellationToken cancellationToken);

        Task DeleteByUser(string userId, CancellationToken cancellationToken);

        Task Update(PendingMembershipChangeDao pendingMembershipChange, CancellationToken cancellationToken);
    }
}
