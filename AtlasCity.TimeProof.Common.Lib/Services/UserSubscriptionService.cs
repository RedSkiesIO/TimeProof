using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Responses;
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

        public UserSubscriptionService(
            ILogger logger,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IPricePlanRepository pricePlanRepository,
            IPaymentService paymentService)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(userRepository);
            AtlasGuard.IsNotNull(paymentRepository);
            AtlasGuard.IsNotNull(pricePlanRepository);
            AtlasGuard.IsNotNull(paymentService);

            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _pricePlanRepository = pricePlanRepository;
            _paymentService = paymentService;
        }
        public async Task<PaymentIntentResponse> GetPaymentIntent(string userId, string pricePlanId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);
            AtlasGuard.IsNotNullOrWhiteSpace(pricePlanId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            var pricePlan = await _pricePlanRepository.GetPricePlanById(pricePlanId, cancellationToken);
            if (pricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{pricePlanId}' identifier.");

            var paymentIntent = await _paymentService.GetPaymentIntentByCustomerId(user.PaymentCustomerId, cancellationToken);

            if (paymentIntent != null && paymentIntent.Amount != pricePlan.Price)
                throw new SubscriptionException($"Payment Intent already exists with mismatch amount. Expected '{pricePlan.Price}' and actual '{paymentIntent.Amount}'");

            if (paymentIntent == null)
                paymentIntent = await _paymentService.CreatePaymentIntent(user.PaymentCustomerId, pricePlan.Price, cancellationToken);

            return paymentIntent.ToResponse();
        }

        public async Task ProcessPayment(string userId, string paymentIntentId, string pricePlanId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);
            AtlasGuard.IsNotNullOrWhiteSpace(paymentIntentId);
            AtlasGuard.IsNotNullOrWhiteSpace(pricePlanId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            var pricePlan = await _pricePlanRepository.GetPricePlanById(pricePlanId, cancellationToken);
            if (pricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{user.PricePlanId}' identifier.");

            var paymentIntent = await _paymentService.GetPaymentIntent(paymentIntentId, cancellationToken);

            if (pricePlan.Price != paymentIntent.Amount)
                _logger.Error($"Payment amount is not matching with price plan '{pricePlan.Id}'. Expected '{pricePlan.Price}' and actual '{paymentIntent.Amount}'");

            _logger.Information($"Collected payment of '{paymentIntent.Amount}' in minimum unit for user '{user.Id}'.");

            await _paymentRepository.CreatePaymentReceived(paymentIntent, cancellationToken);

            user.PricePlanId = pricePlan.Id;
            user.PaymentIntentId = paymentIntentId;
            user.MembershipStartDate = DateTime.UtcNow;
            await _userRepository.UpdateUser(user, cancellationToken);
        }

        public async Task<SetupIntentResponse> CreateSetupIntent(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            var setupIntent = await _paymentService.CreateSetupIntent(user.PaymentCustomerId, cancellationToken);

            return setupIntent.ToResponse();
        }

        public async Task<PaymentMethodResponse> GetCustomerPaymentMethod(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            var payemtnMethod = await _paymentService.GetCustomerPaymentMethod(user.PaymentCustomerId, cancellationToken);

            return payemtnMethod.ToResponse();
        }

        public async Task UpgradePricePlan(string userId, string pricePlanId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("User does not exists.");

            if (user.PricePlanId.Equals(pricePlanId))
            {
                _logger.Warning($"Price plan already in target plan '{pricePlanId}'");
                return;
            }

            var pricePlan = await _pricePlanRepository.GetPricePlanById(pricePlanId, cancellationToken);
            if (pricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{user.PricePlanId}' identifier.");

            var paymentIntent = await _paymentService.CollectPayment(user.PaymentCustomerId, pricePlan.Price, cancellationToken);

            await ProcessPayment(user.Id, paymentIntent.Id, pricePlan.Id, cancellationToken);
        }
    }
}