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
        private readonly SetupIntentService _setupIntentService;

        public StripePaymentService(PaymentIntentService paymentIntentService, CustomerService customerService, SetupIntentService setupIntentService)
        {
            Guard.Argument(paymentIntentService, nameof(paymentIntentService)).NotNull();
            Guard.Argument(customerService, nameof(paymentIntentService)).NotNull();
            Guard.Argument(setupIntentService, nameof(setupIntentService)).NotNull();

            _paymentIntentService = paymentIntentService;
            _customerService = customerService;
            _setupIntentService = setupIntentService;
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

        public async Task<string> CreatePaymentCustomer(Abstractions.DAO.UserDao user, CancellationToken cancellationToken)
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

        public async Task<Abstractions.DAO.UserDao> GetCustomerById(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(paymentCustomerId);

            var customer = await _customerService.GetAsync(paymentCustomerId, null, null, cancellationToken);

            if (customer != null)
                return customer.ToUserDao();

            throw new PaymentException($"Unable to find customer with '{paymentCustomerId}' identifier in stripe payment system.");
        }

        public async Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(setupIntentId);

            var setupIntent = await _setupIntentService.GetAsync(setupIntentId, null, null, cancellationToken);

            if (setupIntent != null)
                return setupIntent.ToSetupIntentDao();

            throw new PaymentException($"Unable to find setup intent with '{setupIntentId}' identifier in stripe payment system.");
        }

        public async Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(paymentCustomerId);

            var options = new SetupIntentCreateOptions { Customer = paymentCustomerId };

            var newSetupIntent = await _setupIntentService.CreateAsync(options, null, cancellationToken);

            if (newSetupIntent != null)
                return newSetupIntent.ToSetupIntentDao();

            throw new PaymentException($"Unable to create customer with customer identifier '{paymentCustomerId}' in stripe payment system.");
        }
    }
}