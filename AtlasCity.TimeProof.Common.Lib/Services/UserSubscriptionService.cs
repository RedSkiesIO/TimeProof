using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;
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

        public async Task<SetupIntentDao> CreateSetupIntent(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

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
                user.SetupIntentId = setupIntent.Id;

                await _userRepository.UpdateUser(user, cancellationToken);
            }

            return setupIntent;
        }

        public async Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(payment);
            AtlasGuard.IsNotNullOrWhiteSpace(payment.UserId);
            AtlasGuard.IsNotNullOrWhiteSpace(payment.PricePlanId);

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

            PaymentResponseDao paymentResponse = null;

            if (!pricePlan.Title.Equals(Constants.FreePricePlanTitle))
            {
                try
                {
                    paymentResponse = await _paymentService.ProcessPayment(payment, user.PaymentCustomerId, cancellationToken);
                    _logger.Information($"Collected payment of '{payment.Amount}' in minimum unit for user '{user.Id}'.");

                    await _paymentRepository.CreatePaymentReceived(paymentResponse, cancellationToken);

                }
                catch (PaymentServiceException ex)
                {
                    _logger.Error(ex, $"Unable to collect payment for user '{user.Id}'.");
                    throw ex;
                }
            }

            user.PricePlanId = payment.PricePlanId;
            user.MembershipStartDate = DateTime.UtcNow;
            await _userRepository.UpdateUser(user, cancellationToken);

            return paymentResponse;
        }

        public async Task<PaymentMethodDao> GetCustomerPaymentMethod(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");


            return await _paymentService.GetCustomerPaymentMethod(user.PaymentCustomerId, cancellationToken);
        }
    }
}