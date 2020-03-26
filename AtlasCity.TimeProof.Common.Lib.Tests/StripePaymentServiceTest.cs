using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Unit.Tests
{
    [TestClass]
    public class StripePaymentServiceTest
    {
        Mock<PaymentIntentService> paymentIntentServiceMock;
        Mock<CustomerService> customerServiceMock;
        Mock<SetupIntentService> setupIntentServiceMock;

        StripePaymentService stripePaymentService;

        [TestInitialize]
        public void Setup()
        {
            paymentIntentServiceMock = new Mock<PaymentIntentService>();
            customerServiceMock = new Mock<CustomerService>();
            setupIntentServiceMock = new Mock<SetupIntentService>();

            stripePaymentService = new StripePaymentService(paymentIntentServiceMock.Object, customerServiceMock.Object, setupIntentServiceMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_StripePaymentService_With_Null_PaymentIntentCreateOptions_Should_Throw_An_Exception()
        {
            new StripePaymentService(null, customerServiceMock.Object, setupIntentServiceMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_StripePaymentService_With_Null_CustomerService_Should_Throw_An_Exception()
        {
            new StripePaymentService(paymentIntentServiceMock.Object, null, setupIntentServiceMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_StripePaymentService_With_Null_SetupIntentService_Should_Throw_An_Exception()
        {
            new StripePaymentService(paymentIntentServiceMock.Object, customerServiceMock.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_ProcessPayment_With_Null_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_ProcessPayment_With_Null_PaymentCustomerId_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = null }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ProcessPayment_With_Empty_PaymentCustomerId_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = string.Empty }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ProcessPayment_With_WhiteSpace_PaymentCustomerId_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = " " }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_ProcessPayment_With_Null_Email_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = "TestId", Email = null }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ProcessPayment_With_Empty_Email_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = "TestId", Email = string.Empty }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ProcessPayment_With_WhiteSpace_Email_In_PaymentDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = "TestId", Email = " " }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_ProcessPayment_With_Valid_PaymentDao_Should_Not_Throw_An_Exception()
        {
            paymentIntentServiceMock.Setup(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PaymentIntent { StripeResponse = new StripeResponse(HttpStatusCode.OK, null, null) }));
            
            var response = stripePaymentService.ProcessPayment(new PaymentDao() { PaymentCustomerId = "TestId", Email = "test@example.com" }, CancellationToken.None).GetAwaiter().GetResult();
            
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_CreatePaymentCustomer_With_Null_UserDao_Should_Throw_An_Exception()
        {
            stripePaymentService.ProcessPayment(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_CreatePaymentCustomer_With_Null_Email_In_UserDao_Should_Throw_An_Exception()
        {
            stripePaymentService.CreatePaymentCustomer(new UserDao { Email = null }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_CreatePaymentCustomer_With_Empty_Email_In_UserDao_Should_Throw_An_Exception()
        {
            stripePaymentService.CreatePaymentCustomer(new UserDao { Email = string.Empty }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_CreatePaymentCustomer_With_WhiteSpace_Email_In_UserDao_Should_Throw_An_Exception()
        {
           
            stripePaymentService.CreatePaymentCustomer(new UserDao { Email = " " }, CancellationToken.None).GetAwaiter().GetResult();
        }
       
        [TestMethod]
        [ExpectedException(typeof(PaymentException))]
        public void Calling_CreatePaymentCustomer_With_Valid_UserDao_And_Payment_CustomerService_Return_Null_Should_Throw_An_Exception()
        {
            customerServiceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((Customer)null));

            stripePaymentService.CreatePaymentCustomer(new UserDao { Email = "test@example.com" }, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_CreatePaymentCustomer_With_Valid_UserDao_And_Payment_CustomerService_Return_Valid_Response_Should_Not_Throw_An_Exception()
        {
            customerServiceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Customer {Id = "testIdentifer" }));

            var userIdentifier = stripePaymentService.CreatePaymentCustomer(new UserDao { Email = "test@example.com" }, CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreEqual("testIdentifer", userIdentifier);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetCustomerById_With_Null_CustomerId_Should_Throw_An_Exception()
        {
            stripePaymentService.GetCustomerById(null, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetCustomerById_With_Empty_CustomerId_Should_Throw_An_Exception()
        {
            stripePaymentService.GetCustomerById(string.Empty, CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetCustomerById_With_WhiteSpace_CustomerId_Should_Throw_An_Exception()
        {
            stripePaymentService.GetCustomerById(" ", CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(PaymentException))]
        public void Calling_GetCustomerById_With_Valid_UserDao_And_Payment_CustomerService_Return_Null_Should_Throw_An_Exception()
        {
            customerServiceMock.Setup(s => s.GetAsync("testCustomerIdentifier", It.IsAny<CustomerGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((Customer)null));

            stripePaymentService.GetCustomerById("testCustomerIdentifier", CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_GetCustomerById_With_Valid_UserDao_And_Payment_CustomerService_Return_Valid_Response_Should_Not_Throw_An_Exception()
        {
            customerServiceMock.Setup(s => s.GetAsync("testCustomerIdentifier", It.IsAny<CustomerGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Customer { Id = "testCustomerIdentifier", Email = "test@example.com", 
                    Address = new Address { Line1 = "testline1", Line2 = "testLine2", City = "testCity", State = "testSate", PostalCode = "testPostcode", Country = "testCountry" } }));

            var user = stripePaymentService.GetCustomerById("testCustomerIdentifier", CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(user);
            Assert.AreEqual("testCustomerIdentifier", user.PaymentCustomerId);
            Assert.AreEqual("test@example.com", user.Email);
           
            Assert.IsNotNull(user.Address);
            Assert.AreEqual("testline1", user.Address.Line1);
            Assert.AreEqual("testLine2", user.Address.Line2);
            Assert.AreEqual("testCity", user.Address.City);
            Assert.AreEqual("testSate", user.Address.State);
            Assert.AreEqual("testPostcode", user.Address.Postcode);
            Assert.AreEqual("testCountry", user.Address.Country);
        }
    }
}
