using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly CustomerService _customerService;

        public StripePaymentService(PaymentIntentService paymentIntentService, CustomerService customerService)
        {
            Guard.Argument(paymentIntentService, nameof(paymentIntentService)).NotNull();
            Guard.Argument(customerService, nameof(paymentIntentService)).NotNull();

            _paymentIntentService = paymentIntentService;
            _customerService = customerService;
        }

        public async Task<string> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken)
        {
            Guard.Argument(payment, nameof(payment)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(payment.PaymentCustomerId);
            AtlasGuard.IsNullOrWhiteSpace(payment.Email);

            var options = new PaymentIntentCreateOptions
            {
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethodTypes = payment.PaymentMethodTypes,
                ReceiptEmail = payment.Email,
                Customer = payment.PaymentCustomerId
            };

            var response = await _paymentIntentService.CreateAsync(options, null, cancellationToken);

            return response.StripeResponse.ToString();
        }

        public async Task<string> CreatePaymentCustomer(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(user.Email);

            var options = new CustomerCreateOptions
            {
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            };

            var newStripeCustomer = await _customerService.CreateAsync(options, null, cancellationToken);
            if (newStripeCustomer != null)
                return newStripeCustomer.Id;

            throw new PaymentException($"Unable to create customer with email '{options.Email}' in stripe payment system.");
        }

        public async Task<UserDao> GetCustomerById(string stripeCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(stripeCustomerId);

            var customer = await _customerService.GetAsync(stripeCustomerId, null, null, cancellationToken);

            if (customer != null)
                return customer.ToUserDao();

            throw new PaymentException($"Unable to find customer with '{stripeCustomerId}' identifier in stripe payment system.");
        }
    }
}