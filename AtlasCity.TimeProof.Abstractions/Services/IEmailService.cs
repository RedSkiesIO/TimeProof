using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendEmail(EmailDao emailDetails, CancellationToken cancellationToken);

        Task SendEmail(EmailDao emailDetails, string filePath, CancellationToken cancellationToken);
    }
}