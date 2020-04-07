using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Extensions;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EmailTemplateHelper : IEmailTemplateHelper
    {
        private readonly string _loginUri;

        private const string EmailTemplateFolder = "EmailTemplates";
        private const string WelcomeEmailFileName = "Welcome.html";


        private const string FullNamePlaceHolder = "[FullNamePlaceHolder]";
        private const string LoginLinkPlaceHolder = "[LoginLinkPlaceHolder]";

        public EmailTemplateHelper(string loginUri)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(loginUri);

            _loginUri = loginUri;
        }

        public async Task<string> GetWelcomeEmailBody(string userFullName, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userFullName);
            var fileText = LoadFileText(WelcomeEmailFileName);

            fileText = fileText.Replace(FullNamePlaceHolder, userFullName);
            fileText = fileText.Replace(LoginLinkPlaceHolder, _loginUri);

            return fileText;
        }

        private string LoadFileText(string fileName)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(fileName);

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), EmailTemplateFolder, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Missing file at {filePath}.");

            return File.ReadAllText(filePath);
        }
    }
}
