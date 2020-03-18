using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;

        public PaymentService (string paymentApiKey)
        {
            _paymentIntentService = new PaymentIntentService(new StripeClient(paymentApiKey));
        }

        public async Task<string> TakePayment(PaymentDao payment)
        {
            Guard.Argument(payment, nameof(payment)).NotNull("User identifier is missing for retrieving timestamps.");
            Guard.Argument(payment.UserId, nameof(payment.UserId)).NotWhiteSpace("User identifier is missing for taking payment.");
            Guard.Argument(payment.Email, nameof(payment.Email)).NotWhiteSpace("User email is missing for taking payment.");

            var options = new PaymentIntentCreateOptions
            {
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethodTypes = payment.PaymentMethodTypes,
                ReceiptEmail = payment.Email,
                Customer = payment.UserId                
            };

            var response = await _paymentIntentService.CreateAsync(options);
            
            return response.StripeResponse.ToString();
        }
    }
}
