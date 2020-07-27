using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using System;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Helpers
{
    [TestClass]
    public class InvoiceHelperTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IEmailTemplateHelper> _emailTemplateHelperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IFileHelper> _fileHelperMock;

        private IInvoiceHelper _invoiceHelper;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _emailTemplateHelperMock = new Mock<IEmailTemplateHelper>();
            _emailServiceMock = new Mock<IEmailService>();
            _fileHelperMock = new Mock<IFileHelper>();

            _invoiceHelper = new InvoiceHelper(_loggerMock.Object, _emailTemplateHelperMock.Object,
                _emailServiceMock.Object, _fileHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_InvoiceHelper_With_Null_Logger_Should_Throw_An_Exception()
        {
            new InvoiceHelper(null, _emailTemplateHelperMock.Object, _emailServiceMock.Object, _fileHelperMock.Object);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_1_ShouldReturn_AA00001()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(1);

            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}AA0001", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_9999_ShouldReturn_AA9999()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(9999);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}AA9999", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_10001_ShouldReturn_AB0001()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(10001);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}AB0001", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_260001_ShouldReturn_BA0001()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(260001);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}BA0001", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_896847_ShouldReturn_DK6847()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(896847);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}DL6847", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_6759999_ShouldReturn_ZZ9999()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(6759999);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}ZZ9999", actualValue);
        }

        [TestMethod]
        public void Calling_CalculateInvoiceNumber_With_6760000_Should_Reset_To_AA00001()
        {
            var actualValue = _invoiceHelper.CalculateInvoiceNumber(6760000);
            Assert.AreEqual($"TS{DateTime.UtcNow:yyMMdd}AA0001", actualValue);
        }
    }
}