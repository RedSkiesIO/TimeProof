using AtlasCity.TimeProof.Abstractions.DAO;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IInvoiceNumberRepository
    {
        public Task<InvoiceNumberDao> GetNextInvoiceNumber(CancellationToken cancellationToken);
    }
}
