using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Extensions;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class EmailTemplateHelper : IEmailTemplateHelper
    {
        private readonly string _loginUri;

        public EmailTemplateHelper(string loginUri)
        {
            AtlasGuard.IsNullOrWhiteSpace(loginUri);

            _loginUri = loginUri;
        }

        public async Task<string> GetWelcomeEmailBody(string userFullName, CancellationToken cancellationToken)
        {
            return "Test";
            //throw new NotImplementedException();
        }
    }
}
