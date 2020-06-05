using System;
using System.Threading;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtlasCity.TimeProof.Common.Lib.Unit.Tests.Helpers
{
    [TestClass]
    public class EmailTemplateHelperTest
    {
        IEmailTemplateHelper emailTemplateHelper;

        [TestInitialize]
        public void Setup()
        {
            emailTemplateHelper = new EmailTemplateHelper("www.contoso.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EmailTemplateHelper_With_Null_LoginUri_Should_Throw_An_Exception()
        {
            new EmailTemplateHelper(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initilising_EmailTemplateHelper_With_Empty_LoginUri_Should_Throw_An_Exception()
        {
            new EmailTemplateHelper(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initilising_EmailTemplateHelper_With_WhiteSpace_LoginUri_Should_Throw_An_Exception()
        {
            new EmailTemplateHelper(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetWelcomeEmailBody_With_Null_Full_Should_Throw_An_Exception()
        {
            emailTemplateHelper.GetWelcomeEmailBody(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_Empty_Full_Should_Throw_An_Exception()
        {
            emailTemplateHelper.GetWelcomeEmailBody(string.Empty, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_WhiteSpace_Full_Should_Throw_An_Exception()
        {
            emailTemplateHelper.GetWelcomeEmailBody(" ", CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_GetWelcomeEmailBody_With_Valid_Parameter_Should_Not_Throw_An_Exception_And_Contains_Valid_Text()
        {
            var welcomeEmailBody = emailTemplateHelper.GetWelcomeEmailBody("Test First Name", CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsTrue(welcomeEmailBody.Contains("Test First Name"));
            Assert.IsTrue(welcomeEmailBody.Contains("www.contoso.com"));
        }
    }
}