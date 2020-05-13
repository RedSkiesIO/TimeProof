using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface  IMembershipRenewRepository
    {
        Task<MembershipRenewDao> AddMembershipRenew(MembershipRenewDao membershipRenew, CancellationToken cancellationToken);

        Task<IEnumerable<MembershipRenewDao>> GetMembershipRenewByDate(DateTime renewDate, CancellationToken cancellationToken);

        Task DeleteMembershipRenew(string id, CancellationToken cancellationToken);
    }
}
