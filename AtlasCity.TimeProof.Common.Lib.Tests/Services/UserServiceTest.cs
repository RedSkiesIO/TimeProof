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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IKeyRepository> _keyRepositoryMock;
        private Mock<IPricePlanRepository> _pricePlanRepositoryMock;
        private Mock<IPaymentService> _paymentServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IEmailTemplateHelper> _emailTemplateHelperMock;

        private UserService _userService;

        readonly CancellationToken _cancellationToken = new CancellationToken();

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _keyRepositoryMock = new Mock<IKeyRepository>();
            _pricePlanRepositoryMock = new Mock<IPricePlanRepository>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateHelperMock = new Mock<IEmailTemplateHelper>();

            _userService = new UserService(_loggerMock.Object, _userRepositoryMock.Object, _keyRepositoryMock.Object,
                _pricePlanRepositoryMock.Object, _paymentServiceMock.Object, _emailServiceMock.Object,
                _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_ILogger_Should_Throw_An_Exception()
        {
            new UserService(null, _userRepositoryMock.Object, _keyRepositoryMock.Object,
                _pricePlanRepositoryMock.Object, _paymentServiceMock.Object, _emailServiceMock.Object,
                _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IUserRepository_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, null, _keyRepositoryMock.Object, _pricePlanRepositoryMock.Object,
                _paymentServiceMock.Object, _emailServiceMock.Object, _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IKeyRepository_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, _userRepositoryMock.Object, null, _pricePlanRepositoryMock.Object,
                _paymentServiceMock.Object, _emailServiceMock.Object, _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IPricePlanRepository_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, _userRepositoryMock.Object, _keyRepositoryMock.Object, null,
                _paymentServiceMock.Object, _emailServiceMock.Object, _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IPaymentService_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, _userRepositoryMock.Object, _keyRepositoryMock.Object,
                _pricePlanRepositoryMock.Object, null, _emailServiceMock.Object, _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IEmailService_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, _userRepositoryMock.Object, _keyRepositoryMock.Object,
                _pricePlanRepositoryMock.Object, _paymentServiceMock.Object, null, _emailTemplateHelperMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_UserService_With_Null_IEmailTemplateHelper_Should_Throw_An_Exception()
        {
            new UserService(_loggerMock.Object, _userRepositoryMock.Object, _keyRepositoryMock.Object,
                _pricePlanRepositoryMock.Object, _paymentServiceMock.Object, _emailServiceMock.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Calling_CreateUser_With_Null_User_Should_Throw_An_Exception()
        {
            await _userService.CreateUser(null, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Calling_CreateUser_With_Null_Email_In_User_Should_Throw_An_Exception()
        {
            await _userService
                .CreateUser(
                    new UserDao
                    {
                        Email = null, FirstName = "Test First Name", LastName = "Test Last Name",
                        PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId",
                        Address = new AddressDao()
                    }, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Calling_CreateUser_With_Empty_Email_In_User_Should_Throw_An_Exception()
        {
            await _userService
                .CreateUser(
                    new UserDao
                    {
                        Email = string.Empty, FirstName = "Test First Name", LastName = "Test Last Name",
                        PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId",
                        Address = new AddressDao()
                    }, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Calling_CreateUser_With_WhiteSpace_Email_In_User_Should_Throw_An_Exception()
        {
            await _userService
                .CreateUser(
                    new UserDao
                    {
                        Email = " ", FirstName = "Test First Name", LastName = "Test Last Name",
                        PaymentCustomerId = "TestPaymentCustomerId", SetupIntentId = "TestSetupIntentId",
                        Address = new AddressDao()
                    }, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(UserException))]

        public async Task Calling_CreateUser_With_Mismatch_User_And_PaymentService_Email_Should_Throw_An_Exception()
        {
            var userId = Guid.NewGuid().ToString();
            const string paymentCustomerId = "testPaymentCustomerId";
            var exception = new PaymentServiceException("Unable to find the customer");

            _userRepositoryMock.Setup(s => s.GetUserById(userId, _cancellationToken)).Returns(Task.FromResult(new UserDao()));

            _paymentServiceMock.Setup(s => s.GetCustomerById(paymentCustomerId, _cancellationToken)).Returns(Task.FromResult(new PaymentCustomerDao {Id = paymentCustomerId, Email = "mismatch@example.com"}));
            _pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, _cancellationToken)).Returns(Task.FromResult(new PricePlanDao{ Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0}));

            await _userService
                .CreateUser(
                    new UserDao
                    {
                        Id = userId, Email = "test@example.com", FirstName = "Test First Name",
                        LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId,
                        SetupIntentId = "TestSetupIntentId", Address = new AddressDao()
                    }, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(PaymentServiceException))]
        public async Task Calling_CreateUser_With_Existing_PaymentCustomerId_And_PaymentService_Throws_Exception_Should_Be_Catched_And_Logged()
        {
            const string paymentCustomerId = "testPaymentCustomerId";
            var exception = new PaymentServiceException("Unable to find the customer");
            var user = new UserDao
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@example.com",
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                PaymentCustomerId = paymentCustomerId,
                SetupIntentId = "TestSetupIntentId",
                Address = new AddressDao()
            };

            _userRepositoryMock.Setup(s => s.GetUserById(user.Id, _cancellationToken)).Returns(Task.FromResult((UserDao) null));
            _paymentServiceMock.Setup(s => s.CreatePaymentCustomer(It.IsAny<UserDao>(), _cancellationToken)).Throws(exception);
            _pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, _cancellationToken)).Returns(Task.FromResult(new PricePlanDao{ Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0}));

            await _userService.CreateUser(user, _cancellationToken);

            _loggerMock.Verify(s => s.Warning(exception, exception.Message), Times.Once);
            _userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserDao>(t => t.CurrentPricePlanId.Equals("freePricePlanTestId")), _cancellationToken), Times.Once);
        }

        [TestMethod]
        public async Task Calling_CreateUser_Happy_Path()
        {
            const string paymentCustomerId = "testPaymentCustomerId";
            var user = new UserDao
            {
                Id = Guid.NewGuid().ToString(), Email = "test@example.com", FirstName = "Test First Name",
                LastName = "Test Last Name", PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId",
                Address = new AddressDao()
            };

            _userRepositoryMock.Setup(s => s.GetUserById(user.Id, _cancellationToken)).Returns(Task.FromResult((UserDao) null));
            _paymentServiceMock.Setup(s => s.GetCustomerById(paymentCustomerId, _cancellationToken)).Returns(Task.FromResult(new PaymentCustomerDao { Id = paymentCustomerId, Email = "test@example.com" }));
            _pricePlanRepositoryMock.Setup(s => s.GetPricePlanByTitle(Constants.FreePricePlanTitle, _cancellationToken)).Returns(Task.FromResult(new PricePlanDao{ Id = "freePricePlanTestId", Title = Constants.FreePricePlanTitle, Price = 0 }));
            _emailTemplateHelperMock.Setup(s => s.GetWelcomeEmailBody(user.FullName, _cancellationToken)).Returns(Task.FromResult("Test Email Body"));

            await _userService.CreateUser(user, _cancellationToken);

            _userRepositoryMock.Verify(s => s.CreateUser(It.Is<UserDao>(t =>t.Email.Equals(user.Email) && t.FirstName.Equals(user.FirstName) && t.LastName.Equals(user.LastName) && t.CurrentPricePlanId.Equals("freePricePlanTestId")), _cancellationToken), Times.Once);
            _loggerMock.Verify(s => s.Information(It.Is<string>(t => t.Contains(user.Email))), Times.AtLeastOnce);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Calling_DeleteUser_With_Null_UserId_Should_Throw_An_Exception()
        {
            await _userService.DeleteUser(null, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Calling_DeleteUser_With_Empty_UserId_Should_Throw_An_Exception()
        {
            await _userService.DeleteUser(string.Empty, _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Calling_DeleteUser_With_WhiteSpace_UserId_Should_Throw_An_Exception()
        {
            await _userService.DeleteUser(" ", _cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(UserException))]
        public async Task Calling_DeleteUser_Where_User_Does_Not_Exists_In_Repository_Should_Throw_An_Exception()
        {
            const string testUserId = "testUserId";
            _userRepositoryMock.Setup(s => s.GetUserById(testUserId, _cancellationToken)).Returns(Task.FromResult((UserDao) null));

            await _userService.DeleteUser(testUserId, _cancellationToken);
        }

        [TestMethod]
        public async Task Calling_DeleteUser_Happy_Path()
        {
            const string testUserId = "testUserId";
            const string paymentCustomerId = "testPaymentCustomerId";
            var testUser = new UserDao
            {
                Id = testUserId, Email = "test@example.com", FirstName = "Test First Name", LastName = "Test Last Name",
                PaymentCustomerId = paymentCustomerId, SetupIntentId = "TestSetupIntentId", Address = new AddressDao()
            };

            _userRepositoryMock.Setup(s => s.GetUserById(testUserId, _cancellationToken)) .Returns(Task.FromResult(testUser));

            await _userService.DeleteUser(testUserId, _cancellationToken);

            _paymentServiceMock.Verify(s => s.DeletePaymentCustomer(paymentCustomerId, _cancellationToken), Times.Once);
            _userRepositoryMock.Verify(s => s.DeleteUser(testUserId, _cancellationToken), Times.Once);
            _loggerMock.Verify(s => s.Information(It.Is<string>(t => t.Contains(testUserId) && t.Contains(testUser.Email))), Times.Once);
        }
    }
}