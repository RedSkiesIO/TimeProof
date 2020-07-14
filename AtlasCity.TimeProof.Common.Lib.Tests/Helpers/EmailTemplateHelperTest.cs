using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Helpers
{
    [TestClass]
    public class EmailTemplateHelperTest
    {
        IEmailTemplateHelper _emailTemplateHelper;

        [TestInitialize]
        public void Setup()
        {
            _emailTemplateHelper = new EmailTemplateHelper("www.contoso.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EmailTemplateHelper_With_Null_LoginUri_Should_Throw_An_Exception()
        {
            _emailTemplateHelper = new EmailTemplateHelper(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initilising_EmailTemplateHelper_With_Empty_LoginUri_Should_Throw_An_Exception()
        {
            _emailTemplateHelper = new EmailTemplateHelper(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initilising_EmailTemplateHelper_With_WhiteSpace_LoginUri_Should_Throw_An_Exception()
        {
            _emailTemplateHelper = new EmailTemplateHelper(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetWelcomeEmailBody_With_Null_RootPath_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody(null, "Test Full Name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_Empty_RootPath_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody(string.Empty, "Test Full Name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_WhiteSpace_RootPath_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody(" ", "Test Full Name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetWelcomeEmailBody_With_Null_FullName_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody("Test Root Path", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_Empty_FullName_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody("Test Root Path", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetWelcomeEmailBody_With_WhiteSpace_FullName_Should_Throw_An_Exception()
        {
            _emailTemplateHelper.GetWelcomeEmailBody("Test Root Path", " ");
        }

        [TestMethod]
        public void Calling_GetWelcomeEmailBody_With_Valid_Parameter_Should_Not_Throw_An_Exception_And_Contains_Valid_Text()
        {
            var welcomeEmailBody = _emailTemplateHelper.GetWelcomeEmailBody(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Test First Name");

            Assert.IsTrue(welcomeEmailBody.Contains("Test First Name"));
            Assert.IsTrue(welcomeEmailBody.Contains("www.contoso.com"));
        }
    }
}