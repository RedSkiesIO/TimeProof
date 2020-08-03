using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;
using System.IO;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EmailTemplateHelper : IEmailTemplateHelper
    {
        private readonly string _loginUri;

        private const string WelcomeEmailFileName = "Welcome.html";
        private const string InvoiceEmailFileName = "Invoice.html";

        private const string FirstNamePlaceHolder = "[FirstNamePlaceHolder]";
        private const string LoginLinkPlaceHolder = "[LoginLinkPlaceHolder]";

        public EmailTemplateHelper(string loginUri)
        {
            Guard.Argument(loginUri, nameof(loginUri)).NotNull().NotEmpty().NotWhiteSpace();

            _loginUri = loginUri;
        }

        public string GetWelcomeEmailBody(string rootFolder, string userFirstName)
        {
            Guard.Argument(rootFolder, nameof(rootFolder)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(userFirstName, nameof(userFirstName)).NotNull().NotEmpty().NotWhiteSpace();

            var fileText = LoadFileText(rootFolder, WelcomeEmailFileName);
            fileText = fileText.Replace(FirstNamePlaceHolder, userFirstName);
            fileText = fileText.Replace(LoginLinkPlaceHolder, _loginUri);

            return fileText;
        }

        public string GetInvoiceEmailBody(string rootFolder, string userFirstName)
        {
            Guard.Argument(rootFolder, nameof(rootFolder)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(userFirstName, nameof(userFirstName)).NotNull().NotEmpty().NotWhiteSpace();

            var fileText = LoadFileText(rootFolder, InvoiceEmailFileName);
            fileText = fileText.Replace(FirstNamePlaceHolder, userFirstName);
            fileText = fileText.Replace(LoginLinkPlaceHolder, _loginUri);

            return fileText;
        }

        private string LoadFileText(string rootFolder, string fileName)
        {
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty().NotWhiteSpace();

            var filePath = Path.Combine(rootFolder, Constants.TemplateFolder, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Missing file at {filePath}.");

            return File.ReadAllText(filePath);
        }
    }
}