using AtlasCity.TimeProof.Abstractions.DAO;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IKeyRepository
    {
        Task<KeyDao> GetByUserId(string userId, CancellationToken cancellationToken);

        Task<KeyDao> CreateKey(KeyDao key, CancellationToken cancellationToken);
    }
}
