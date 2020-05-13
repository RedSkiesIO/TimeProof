using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEmailTemplateHelper
    {
        Task<string> GetWelcomeEmailBody(string userFullName, CancellationToken cancellationToken);

        Task<string> CreateFileFromText(string fileText, string fileName, CancellationToken cancellationToken);

        void DeleteFileDirectory(string filePath);
    }
}
