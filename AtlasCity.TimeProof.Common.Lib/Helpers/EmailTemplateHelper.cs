using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EmailTemplateHelper : IEmailTemplateHelper
    {
        private readonly string _loginUri;

        private const string EmailTemplateFolder = "EmailTemplates";
        private const string WelcomeEmailFileName = "Welcome.html";


        private const string FirstNamePlaceHolder = "[FirstNamePlaceHolder]";
        private const string LoginLinkPlaceHolder = "[LoginLinkPlaceHolder]";

        public EmailTemplateHelper(string loginUri)
        {
            Guard.Argument(loginUri, nameof(loginUri)).NotNull().NotEmpty().NotWhiteSpace();

            _loginUri = loginUri;
        }

        public async Task<string> GetWelcomeEmailBody(string userFirstName, CancellationToken cancellationToken)
        {
            Guard.Argument(userFirstName, nameof(userFirstName)).NotNull().NotEmpty().NotWhiteSpace();

            var fileText = LoadFileText(WelcomeEmailFileName);
            fileText = fileText.Replace(FirstNamePlaceHolder, userFirstName);
            fileText = fileText.Replace(LoginLinkPlaceHolder, _loginUri);

            return fileText;
        }

        public async Task<string> CreateFileFromText(string fileText, string fileName, CancellationToken cancellationToken)
        {
            Guard.Argument(fileText, nameof(fileText)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty().NotWhiteSpace();

            var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Guid.NewGuid().ToString());

            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            using (var file = new StreamWriter(filePath))
            {
                await file.WriteAsync(fileText);
            }

            return filePath;
        }

        public void DeleteFileDirectory(string filePath)
        {
            Guard.Argument(filePath, nameof(filePath)).NotNull().NotEmpty().NotWhiteSpace();

            var fileInfo = new FileInfo(filePath);
            if (Directory.Exists(fileInfo.DirectoryName))
                Directory.Delete(fileInfo.DirectoryName, true);

        }

        private string LoadFileText(string fileName)
        {
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty().NotWhiteSpace();

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), EmailTemplateFolder, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Missing file at {filePath}.");

            return File.ReadAllText(filePath);
        }
    }
}