using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Common.Lib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Services
{
    [TestClass]
    public class EmailServiceTest
    {
        SmtpClient smtpClient;
        Mock<ILogger> loggerMock;

        EmailService emailService;

        [TestInitialize]
        public void Setup()
        {
            smtpClient = new SmtpClient("smtp.office365.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("noreply@atlascity.io", "on6TSs4SNaVwXUqQQHJaU2dioYxBnS");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            loggerMock = new Mock<ILogger>();

            emailService = new EmailService(smtpClient, loggerMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EmailService_With_Null_SmtpClient_Should_Throw_An_Exception()
        {
            new EmailService(null, loggerMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EmailService_With_Null_ILogger_Should_Throw_An_Exception()
        {
            new EmailService(smtpClient, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_SendEmail_With_Null_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_SendEmail_With_Null_ToAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = null, ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_Empty_ToAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = string.Empty, ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_WhiteSpace_ToAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = " ", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_SendEmail_With_Null_FromAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = null, Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_Empty_FromAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = string.Empty, Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_WhiteSpace_FromAddress_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = " ", Subject = "Test Subject", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_SendEmail_With_Null_Subject_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = null, HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_Empty_Subject_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = string.Empty, HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_WhiteSpace_Subject_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = " ", HtmlBody = "<html><body>Test Body</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_SendEmail_With_Null_Body_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = null }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_Empty_Body_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = string.Empty }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_SendEmail_With_WhiteSpace_Body_In_EmailDao_Should_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "test@example.com", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "Test Subject", HtmlBody = " " }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [Ignore("Don't want to send email every time test is run.")]
        [TestMethod]
        public void Calling_SendEmail_With_Valid_EmailDao_Should_Not_Throw_An_Exception()
        {
            emailService.SendEmail(new EmailDao() { ToAddress = "sudhir.tibrewal@atlascity.io", ToName = "Test Name", FromAddress = "noreply@atlascity.io", Subject = "TimeProof Test", HtmlBody = $"<html><body>TimeProof Test : {DateTime.UtcNow.ToLongDateString()}</body></html>" }, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}