using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPricePlanRepository _pricePlanRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public UserSubscriptionService(
            ILogger logger,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IPricePlanRepository pricePlanRepository,
            IPaymentService paymentService,
            IEmailService emailService)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(userRepository);
            AtlasGuard.IsNotNull(paymentRepository);
            AtlasGuard.IsNotNull(pricePlanRepository);
            AtlasGuard.IsNotNull(paymentService);
            AtlasGuard.IsNotNull(emailService);

            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _pricePlanRepository = pricePlanRepository;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        public async Task<SetupIntentDao> CreateSetupIntent(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(userId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            SetupIntentDao setupIntent = null;

            if (!string.IsNullOrWhiteSpace(user.SetupIntentId))
            {
                setupIntent = await _paymentService.GetSetupIntent(user.SetupIntentId, cancellationToken);
                if (setupIntent != null)
                    return setupIntent;

                _logger.Warning($"Setup intent '{user.SetupIntentId}' is missing from the payment service.");
            }

            if (setupIntent == null)
            {
                setupIntent = await _paymentService.CreateSetupIntent(user.PaymentCustomerId, cancellationToken);
                await _userRepository.AddSetupIntent(user.Email, setupIntent.Id, cancellationToken);
            }

            return setupIntent;
        }

        public async Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(payment);
            AtlasGuard.IsNullOrWhiteSpace(payment.UserId);
            AtlasGuard.IsNullOrWhiteSpace(payment.PricePlanId);

            var user = await _userRepository.GetUserById(payment.UserId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            var pricePlan = await _pricePlanRepository.GetPricePlanById(payment.PricePlanId, cancellationToken);
            if (pricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{payment.PricePlanId}' identifier.");

            if (pricePlan.Price != payment.Amount)
                throw new SubscriptionException($"Processing payment amount is not matching with price plan '{payment.PricePlanId}'. Expected '{pricePlan.Price}' and actual '{payment.Amount}'");



            // TODO: Sudhir if free plan don't call stripe payment and ignore "PaymentMethodId"
            PaymentResponseDao paymentResponse = null;
            try
            {
                paymentResponse = await _paymentService.ProcessPayment(payment, user.PaymentCustomerId, cancellationToken);
                _logger.Information($"Collected payment for user '{user.Id}'.");

                await _paymentRepository.CreatePaymentReceived(paymentResponse, cancellationToken);
                return paymentResponse;
            }
            catch (PaymentServiceException ex)
            {
                _logger.Error(ex, $"Unable to collect payment for user '{user.Id}'.");
                throw ex;
            }

            //TODO: Sudhir If price plan changes, then update the userRepository after collecting payment
        }
    }
}
