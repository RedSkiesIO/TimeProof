using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Responses;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPricePlanRepository _pricePlanRepository;
        private readonly IMembershipRenewRepository _membershipRenewRepository;
        private readonly IPaymentService _paymentService;
        private readonly ISystemDateTime _systemDateTime;

        public UserSubscriptionService(
            ILogger logger,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IPricePlanRepository pricePlanRepository,
            IMembershipRenewRepository membershipRenewRepository,
            IPaymentService paymentService,
            ISystemDateTime systemDateTime)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(paymentRepository, nameof(paymentRepository)).NotNull();
            Guard.Argument(pricePlanRepository, nameof(pricePlanRepository)).NotNull();
            Guard.Argument(membershipRenewRepository, nameof(membershipRenewRepository)).NotNull();
            Guard.Argument(paymentService, nameof(paymentService)).NotNull();
            Guard.Argument(systemDateTime, nameof(systemDateTime)).NotNull();

            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _pricePlanRepository = pricePlanRepository;
            _membershipRenewRepository = membershipRenewRepository;
            _paymentService = paymentService;
            _systemDateTime = systemDateTime;
        }
        public async Task<PaymentIntentResponse> GetPaymentIntent(string userId, string pricePlanId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(pricePlanId, nameof(pricePlanId)).NotNull().NotEmpty().NotWhiteSpace();

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
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(paymentIntentId, nameof(paymentIntentId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(pricePlanId, nameof(pricePlanId)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            var pricePlan = await _pricePlanRepository.GetPricePlanById(pricePlanId, cancellationToken);
            if (pricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{user.CurrentPricePlanId}' identifier.");

            var paymentIntent = await _paymentService.GetPaymentIntent(paymentIntentId, cancellationToken);

            if (pricePlan.Price != paymentIntent.Amount)
                _logger.Error($"Payment amount is not matching with price plan '{pricePlan.Id}'. Expected '{pricePlan.Price}' and actual '{paymentIntent.Amount}'");

            _logger.Information($"Collected payment of '{paymentIntent.Amount}' in minimum unit for user '{user.Id}'.");

            var paymentDao = new ProcessedPaymentDao
            {
                UserId = user.Id,
                PaymentServiceId = paymentIntent.Id,
                Amount = paymentIntent.Amount,
                PricePlanId = pricePlan.Id,
                NoOfStamps = pricePlan.NoOfStamps,
                Created = paymentIntent.Created,
            };

            await _paymentRepository.CreatePaymentReceived(paymentDao, cancellationToken);

            user.CurrentPricePlanId = pricePlan.Id;
            user.RemainingTimeStamps = pricePlan.NoOfStamps;
            user.PaymentIntentId = paymentIntentId;

            user.MembershipRenewDate = _systemDateTime.GetUtcDateTime().AddMonths(1);

            await _userRepository.UpdateUser(user, cancellationToken);
        }

        public async Task<SetupIntentResponse> CreateSetupIntent(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            var setupIntent = await _paymentService.CreateSetupIntent(user.PaymentCustomerId, cancellationToken);

            return setupIntent.ToResponse();
        }

        public async Task<PaymentMethodResponse> GetCustomerPaymentMethod(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            var payemtnMethod = await _paymentService.GetCustomerPaymentMethod(user.PaymentCustomerId, cancellationToken);

            return payemtnMethod.ToResponse();
        }

        public async Task ChangePricePlan(string userId, string pricePlanId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(pricePlanId, nameof(pricePlanId)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("User does not exists.");

            if (user.CurrentPricePlanId.Equals(pricePlanId))
            {
                _logger.Warning($"Price plan already in target plan '{pricePlanId}'");
                return;
            }

            var currentPricePlan = await _pricePlanRepository.GetPricePlanById(user.CurrentPricePlanId, cancellationToken);
            if (currentPricePlan == null)
                throw new SubscriptionException($"Unable to find the current price plan with '{user.CurrentPricePlanId}' identifier for user '{user.Id}'.");

            var newPricePlan = await _pricePlanRepository.GetPricePlanById(pricePlanId, cancellationToken);
            if (newPricePlan == null)
                throw new SubscriptionException($"Unable to find the price plan with '{pricePlanId}' identifier.");

            var isUpgradedPlan = newPricePlan.Price > currentPricePlan.Price;
            var isDowngradedToBasicPlan = newPricePlan.Title.Equals(Constants.FreePricePlanTitle, StringComparison.InvariantCultureIgnoreCase);

            var processPayment = isUpgradedPlan || (user.MembershipRenewDate.Date <= _systemDateTime.GetUtcDateTime().Date && !isDowngradedToBasicPlan);

            if (processPayment)
            {
                var paymentIntent = await _paymentService.CollectPayment(user.PaymentCustomerId, newPricePlan.Price, cancellationToken);

                await ProcessPayment(user.Id, paymentIntent.Id, newPricePlan.Id, cancellationToken);
            }
            else if (user.MembershipRenewDate.Date <= _systemDateTime.GetUtcDateTime().Date && isDowngradedToBasicPlan)
            {
                user.CurrentPricePlanId = newPricePlan.Id;
                user.RemainingTimeStamps = newPricePlan.NoOfStamps;
                user.MembershipRenewDate = _systemDateTime.GetUtcDateTime().AddMonths(1);

                await _userRepository.UpdateUser(user, cancellationToken);
            }
            else
            {
                user.RenewPricePlanId = newPricePlan.Id;
                await _membershipRenewRepository.AddMembershipRenew(new MembershipRenewDao { Id = Guid.NewGuid().ToString(), UserId = user.Id, MembershipRenewDate = user.MembershipRenewDate, NewPricePlanId = newPricePlan.Id }, cancellationToken);

                await _userRepository.UpdateUser(user, cancellationToken);
            }
        }
    }
}