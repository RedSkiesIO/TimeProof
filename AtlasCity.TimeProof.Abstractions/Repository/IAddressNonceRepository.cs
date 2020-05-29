using AtlasCity.TimeProof.Abstractions.DAO;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IAddressNonceRepository
    {
        public Task<NonceDao> GetNonceByAddress(string address, CancellationToken cancellationToken);

        public Task<NonceDao> UpdateAddressNonce(NonceDao nonce, CancellationToken cancellationToken);
    }
}
