using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class FileHelper : IFileHelper
    {
        public async Task<string> CreateFileFromText(string fileText, string fileName, CancellationToken cancellationToken)
        {
            Guard.Argument(fileText, nameof(fileText)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty().NotWhiteSpace();

            var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, Guid.NewGuid().ToString());

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            await using var file = new StreamWriter(filePath);
            await file.WriteAsync(fileText);

            return filePath;
        }

        public void DeleteFileDirectory(string filePath)
        {
            Guard.Argument(filePath, nameof(filePath)).NotNull().NotEmpty().NotWhiteSpace();

            var fileInfo = new FileInfo(filePath);
            if (Directory.Exists(fileInfo.DirectoryName))
                Directory.Delete(fileInfo.DirectoryName, true);

        }
    }
}
