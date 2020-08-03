using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IFileHelper
    {
        Task<string> CreateFileFromText(string fileText, string fileName, CancellationToken cancellationToken);

        void DeleteFileDirectory(string filePath);
    }
}
