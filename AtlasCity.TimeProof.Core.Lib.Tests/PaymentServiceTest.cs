using System;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib;
using Xunit;

namespace AtlasCity.TimeProof.Core.Lib.Tests
{
    public class PaymentServiceTest
    {
        [Fact]
        public void Test1()
        {
            IPaymentService paymentService = new PaymentService("sk_test_cWWzTkeFvCFbKEUsFpe1CRSt00DzkGfYAn");

            var payment = new PaymentDao()
            {
                Amount = 1000,
                Email = "sudhir.tibrewal@atlascity.io",
                UserId = "cus_GvXhHeLd6k3vcn"
            };

            var response = paymentService.TakePayment(payment).GetAwaiter().GetResult();

        }
    }
}
