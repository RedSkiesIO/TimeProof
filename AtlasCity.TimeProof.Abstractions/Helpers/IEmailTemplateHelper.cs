using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEmailTemplateHelper
    {
        Task<string> GetWelcomeEmailBody(string userFullName, CancellationToken cancellationToken);
    }
}
