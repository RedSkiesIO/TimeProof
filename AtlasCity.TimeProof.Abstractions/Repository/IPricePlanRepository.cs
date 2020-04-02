using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IPricePlanRepository
    {
        Task<IEnumerable<PricePlanDao>> GetPricePlans(CancellationToken cancellationToken);

        Task<PricePlanDao> GetPricePlanById(string pricePlanId, CancellationToken cancellationToken);

        Task<PricePlanDao> GetPricePlanByTitle(string pricePlanName, CancellationToken cancellationToken);

        Task<PricePlanDao> AddPricePlans(PricePlanDao pricePlan, CancellationToken cancellationToken);
    }
}
