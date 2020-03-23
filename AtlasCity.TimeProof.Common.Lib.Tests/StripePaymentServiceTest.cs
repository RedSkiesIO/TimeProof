using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Tests
{
    [TestClass]
    public class StripePaymentServiceTest
    {
        Mock<PaymentIntentService> paymentIntentServiceMock;
        Mock<CustomerService> customerServiceMock;

        [TestInitialize]
        public void Setup()
        {
            paymentIntentServiceMock = new Mock<PaymentIntentService>();
            customerServiceMock = new Mock<CustomerService>();
        }

        [TestMethod]
        public void Initilising_StripePaymentService_With_Null_PaymentIntentCreateOptions_Should_Throw_An_Exception()
        {
            new StripePaymentService(null, customerServiceMock.Object);
        }

        [TestMethod]
        public void Initilising_StripePaymentService_With_Null_CustomerService_Should_Throw_An_Exception()
        {
            new StripePaymentService(paymentIntentServiceMock.Object, null);
        }

        [TestMethod]
        public void Initilising_StripePaymentService_With_Crrect_Parameters_Should_Not_Throw_An_Exception()
        {
            new StripePaymentService(paymentIntentServiceMock.Object, customerServiceMock.Object);
        }

        //[TestMethod]
        //public void Test1()
        //{
        //    paymentIntentServiceMock.Setup(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), null, It.IsAny<CancellationToken>())).Returns(new )

        //    IPaymentService paymentService = new StripePaymentService(paymentIntentServiceMock.Object, customerServiceMock.Object);
        //    var payment = new PaymentDao()
        //    {
        //        Amount = 1000,
        //        Email = "sudhir.tibrewal@atlascity.io",
        //        UserId = "cus_GvXhHeLd6k3vcn"
        //    };

        //    paymentService.TakePayment(payment, CancellationToken.None).GetAwaiter().GetResult();

        //}
    }
}
