using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger _logger;

        public EmailService(SmtpClient smtpClient, ILogger logger)
        {
            AtlasGuard.IsNotNull(smtpClient);
            AtlasGuard.IsNotNull(logger);

            _smtpClient = smtpClient;
            _logger = logger;
        }

        public async Task SendEmail(EmailDao emailDetails, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(emailDetails);
            AtlasGuard.IsNullOrWhiteSpace(emailDetails.ToAddress);
            AtlasGuard.IsNullOrWhiteSpace(emailDetails.FromAddress);
            AtlasGuard.IsNullOrWhiteSpace(emailDetails.Subject);
            AtlasGuard.IsNullOrWhiteSpace(emailDetails.HtmlBody);

            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailDetails.ToAddress, emailDetails.ToName));
            mailMessage.From = new MailAddress(emailDetails.FromAddress);
            mailMessage.Subject = emailDetails.Subject;
            mailMessage.Body = emailDetails.HtmlBody;
            mailMessage.IsBodyHtml = true;

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unable to send the email to {emailDetails.ToAddress}.");
                throw ex;
            }
        }
    }
}