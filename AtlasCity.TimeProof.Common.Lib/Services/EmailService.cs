using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger _logger;

        public EmailService(SmtpClient smtpClient, ILogger logger)
        {
            Guard.Argument(smtpClient, nameof(smtpClient)).NotNull();
            Guard.Argument(logger, nameof(logger)).NotNull();

            _smtpClient = smtpClient;
            _logger = logger;
        }

        public async Task SendEmail(EmailDao emailDetails, CancellationToken cancellationToken)
        {
            Guard.Argument(emailDetails, nameof(emailDetails)).NotNull();
            Guard.Argument(emailDetails.ToAddress, nameof(emailDetails.ToAddress)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.FromAddress, nameof(emailDetails.FromAddress)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.Subject, nameof(emailDetails.Subject)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.HtmlBody, nameof(emailDetails.HtmlBody)).NotNull().NotEmpty().NotWhiteSpace();

            var mailMessage = ComposeEmail(emailDetails);

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

        public async Task SendEmail(EmailDao emailDetails, string filePath, CancellationToken cancellationToken)
        {
            Guard.Argument(emailDetails, nameof(emailDetails)).NotNull();
            Guard.Argument(emailDetails.ToAddress, nameof(emailDetails.ToAddress)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.FromAddress, nameof(emailDetails.FromAddress)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.Subject, nameof(emailDetails.Subject)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(emailDetails.HtmlBody, nameof(emailDetails.HtmlBody)).NotNull().NotEmpty().NotWhiteSpace();

            Guard.Argument(filePath, nameof(filePath)).NotNull().NotEmpty().NotWhiteSpace();

            using (var mailMessage = ComposeEmail(emailDetails))
            {
                AddAttachment(mailMessage, filePath);

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

        private MailMessage ComposeEmail(EmailDao emailDetails)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailDetails.ToAddress, emailDetails.ToName));
            mailMessage.From = new MailAddress(emailDetails.FromAddress);
            mailMessage.Subject = emailDetails.Subject;
            mailMessage.Body = emailDetails.HtmlBody;
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        private MailMessage AddAttachment(MailMessage mailMessage, string filePath)
        {
            Guard.Argument(mailMessage, nameof(mailMessage)).NotNull();
            Guard.Argument(filePath, nameof(filePath)).NotNull().NotEmpty().NotWhiteSpace();

            var data = new Attachment(filePath, MediaTypeNames.Text.Plain);
            
            var disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);

            mailMessage.Attachments.Add(data);

            return mailMessage;
        }
    }
}