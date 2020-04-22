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


        private const string FullNamePlaceHolder = "[FullNamePlaceHolder]";
        private const string LoginLinkPlaceHolder = "[LoginLinkPlaceHolder]";

        public EmailTemplateHelper(string loginUri)
        {
            Guard.Argument(loginUri, nameof(loginUri)).NotNull().NotEmpty().NotWhiteSpace();

            _loginUri = loginUri;
        }

        public async Task<string> GetWelcomeEmailBody(string userFullName, CancellationToken cancellationToken)
        {
            Guard.Argument(userFullName, nameof(userFullName)).NotNull().NotEmpty().NotWhiteSpace();

            var fileText = LoadFileText(WelcomeEmailFileName);
            fileText = fileText.Replace(FullNamePlaceHolder, userFullName);
            fileText = fileText.Replace(LoginLinkPlaceHolder, _loginUri);

            return fileText;
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
