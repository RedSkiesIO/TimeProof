namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEmailTemplateHelper
    {
        string GetWelcomeEmailBody(string rootFolder, string userFullName);

        string GetInvoiceEmailBody(string rootFolder, string userFirstName);
    }
}
