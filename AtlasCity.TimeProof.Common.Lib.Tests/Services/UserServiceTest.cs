using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        Mock<ILogger> loggerMock;
        Mock<IUserRepository> userRepositoryMock;
        Mock<IPricePlanRepository> pricePlanRepositoryMock;
        Mock<IPaymentService> paymentServiceMock;
        Mock<IEmailService> emailServiceMock;
        Mock<IEmailTemplateHelper> emailTemplateHelperMock;

        UserService userService;

        CancellationToken cancellationToken = new CancellationToken();

        [TestInitialize]
        public void Setup()
        {
            loggerMock = new Mock<ILogger>();
            userRepositoryMock = new Mock<IUserRepository>();
            pricePlanRepositoryMock = new Mock<IPricePlanRepository>();
            paymentServiceMock = new Mock<IPaymentService>();
            emailServiceMock = new Mock<IEmailService>();
            emailTemplateHelperMock = new Mock<IEmailTemplateHelper>();

            userService = new UserService(loggerMock.Object, userRepositoryMock.Object, pricePlanRepositoryMock.Object, paymentServiceMock.Object, emailServiceMock.Object, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_ILogger_Should_Throw_An_Exception()
        {
            new UserService(null, userRepositoryMock.Object, pricePlanRepositoryMock.Object, paymentServiceMock.Object, emailServiceMock.Object, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IUserRepository_Should_Throw_An_Exception()
        {
            new UserService(loggerMock.Object, null, pricePlanRepositoryMock.Object, paymentServiceMock.Object, emailServiceMock.Object, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IPricePlanRepository_Should_Throw_An_Exception()
        {
            new UserService(loggerMock.Object, userRepositoryMock.Object, null, paymentServiceMock.Object, emailServiceMock.Object, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IPaymentService_Should_Throw_An_Exception()
        {
            new UserService(loggerMock.Object, userRepositoryMock.Object, pricePlanRepositoryMock.Object, null, emailServiceMock.Object, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IEmailService_Should_Throw_An_Exception()
        {
            new UserService(loggerMock.Object, userRepositoryMock.Object, pricePlanRepositoryMock.Object, paymentServiceMock.Object, null, emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IEmailTemplateHelper_Should_Throw_An_Exception()
        {
            new UserService(loggerMock.Object, userRepositoryMock.Object, pricePlanRepositoryMock.Object, paymentServiceMock.Object, emailServiceMock.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetUserByEmail_With_Null_Email_Should_Throw_An_Exception()
        {
            userService.GetUserByEmail(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetUserByEmail_With_Empty_Email_Should_Throw_An_Exception()
        {
            userService.GetUserByEmail(string.Empty, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetUserByEmail_With_WhiteSpace_Email_Should_Throw_An_Exception()
        {
            userService.GetUserByEmail(" ", cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_GetUserByEmail_With_Valid_Email_Should_Not_Throw_An_Exception()
        {
            var email = "test@example.com";

            userService.GetUserByEmail(email, cancellationToken).GetAwaiter().GetResult();

            userRepositoryMock.Verify(s => s.GetUserByEmail(email, cancellationToken), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_CreateUser_With_Null_User_Should_Throw_An_Exception()
        {
            userService.CreateUser(null, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_CreateUser_With_Null_Email_In_User_Should_Throw_An_Exception()
        {
            userService.CreateUser(new UserDao { Email = null, FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId", Address = new AddressDao() }, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_CreateUser_With_Empty_Email_In_User_Should_Throw_An_Exception()
        {
            userService.CreateUser(new UserDao { Email = string.Empty, FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId", Address = new AddressDao() }, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_CreateUser_With_WhiteSpace_Email_In_User_Should_Throw_An_Exception()
        {
            userService.CreateUser(new UserDao { Email = " ", FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId", Address = new AddressDao() }, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(UserException))]

        public void Calling_CreateUser_With_Mismatch_User_And_PaymentService_Email_Should_Throw_An_Exception()
        {
            var paymentCustomerId = "testPaymentCustomerId";
            var exception = new PaymentServiceException("Unable to find the customer");

            paymentServiceMock.Setup(s => s.GetCustomerById(paymentCustomerId, cancellationToken)).Returns(Task.FromResult(new PaymentCustomerDao { Id = paymentCustomerId, Email = "mismatch@example.com" }));
            pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, cancellationToken)).Returns(Task.FromResult(new PricePlanDao { Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0 }));

            userService.CreateUser(new UserDao { Email = "test@example.com", FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId", Address = new AddressDao() }, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_CreateUser_With_Existing_PaymentCustomerId_And_PaymentService_Throws_Exception_Should_Be_Catched_And_Logged()
        {
            var paymentCustomerId = "testPaymentCustomerId";
            var exception = new PaymentServiceException("Unable to find the customer");

            paymentServiceMock.Setup(s => s.GetCustomerById(paymentCustomerId, cancellationToken)).Throws(exception);
            pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, cancellationToken)).Returns(Task.FromResult(new PricePlanDao { Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0 }));

            userService.CreateUser(new UserDao { Email = "test@example.com", FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId", Address = new AddressDao() }, cancellationToken).GetAwaiter().GetResult();

            loggerMock.Verify(s => s.Warning(exception, exception.Message), Times.Once);
            userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserDao>(t => t.CurrentPricePlanId.Equals("freePricePlanTestId")), cancellationToken), Times.Once);
        }

        [TestMethod]
        public void Calling_CreateUser_Happy_Path()
        {
            var paymentCustomerId = "testPaymentCustomerId";
            var user = new UserDao { Email = "test@example.com", FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId", Address = new AddressDao() };

            paymentServiceMock.Setup(s => s.GetCustomerById(paymentCustomerId, cancellationToken)).Returns(Task.FromResult(new PaymentCustomerDao { Id = paymentCustomerId, Email = "test@example.com" }));
            pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, cancellationToken)).Returns(Task.FromResult(new PricePlanDao { Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0 }));
            emailTemplateHelperMock.Setup(s => s.GetWelcomeEmailBody(user.FullName, cancellationToken)).Returns(Task.FromResult("Test Email Body"));

            userService.CreateUser(user, cancellationToken).GetAwaiter().GetResult();


            userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserDao>(t => t.Email.Equals(user.Email) && t.FirstName.Equals(user.FirstName) && t.LastName.Equals(user.LastName) && t.CurrentPricePlanId.Equals("freePricePlanTestId")), cancellationToken), Times.Once);
            loggerMock.Verify(s => s.Information(It.Is<string>(t => t.Contains(user.Email))), Times.Once);
            emailServiceMock.Verify(s => s.SendEmail(It.Is<EmailDao>(t => t.ToAddress.Equals(user.Email) && t.ToName.Equals(user.FullName) && t.FromAddress.Equals(Constants.AutomatedEmailFromAddress) && t.Subject.Equals(Constants.WelcomeEmailSubject) && t.HtmlBody.Equals("Test Email Body")), cancellationToken), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_DeleteUser_With_Null_UserId_Should_Throw_An_Exception()
        {
            userService.DeleteUser(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_DeleteUser_With_Empty_UserId_Should_Throw_An_Exception()
        {
            userService.DeleteUser(string.Empty, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_DeleteUser_With_WhiteSpace_UserId_Should_Throw_An_Exception()
        {
            userService.DeleteUser(" ", cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(UserException))]
        public void Calling_DeleteUser_Where_User_Does_Not_Exists_In_Repository_Should_Throw_An_Exception()
        {
            var testUserId = "testUserId";
            userRepositoryMock.Setup(s => s.GetUserById(testUserId, cancellationToken)).Returns(Task.FromResult((UserDao)null));

            userService.DeleteUser(testUserId, cancellationToken).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_DeleteUser_Happy_Path()
        {
            var testUserId = "testUserId";
            var paymentCustomerId = "testPaymentCustomerId";
            var testUser = new UserDao { Id = testUserId, Email = "test@example.com", FirstName = "Test First Name", LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId", Address = new AddressDao() };
            userRepositoryMock.Setup(s => s.GetUserById(testUserId, cancellationToken)).Returns(Task.FromResult(testUser));

            userService.DeleteUser(testUserId, cancellationToken).GetAwaiter().GetResult();

            paymentServiceMock.Verify(s => s.DeletePaymentCustomer(paymentCustomerId, cancellationToken), Times.Once);
            userRepositoryMock.Verify(s => s.DeleteUser(testUserId, cancellationToken), Times.Once);
            loggerMock.Verify(s => s.Information(It.Is<string>(t => t.Contains(testUserId) && t.Contains(testUser.Email))), Times.Once);
        }
    }
}