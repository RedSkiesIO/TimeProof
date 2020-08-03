using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Responses;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPricePlanRepository _pricePlanRepository;
        private readonly IPendingMembershipChangeRepository _pendingMembershipChangeRepository;
        private readonly IPaymentService _paymentService;
        private readonly ISystemDateTime _systemDateTime;
        private readonly IInvoiceHelper _invoiceHelper;
        private readonly IInvoiceNumberRepository _invoiceNumberRepository;

        public UserSubscriptionService(
            ILogger logger,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            IPricePlanRepository pricePlanRepository,
            IPendingMembershipChangeRepository pendingMembershipChangeRepository,
            IPaymentService paymentService,
            ISystemDateTime systemDateTime,
            IInvoiceHelper invoiceHelper,
            IInvoiceNumberRepository invoiceNumberRepository
        )
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(paymentRepository, nameof(paymentRepository)).NotNull();
            Guard.Argument(pricePlanRepository, nameof(pricePlanRepository)).NotNull();
            Guard.Argument(pendingMembershipChangeRepository, nameof(pendingMembershipChangeRepository)).NotNull();
            Guard.Argument(paymentService, nameof(paymentService)).NotNull();
            Guard.Argument(systemDateTime, nameof(systemDateTime)).NotNull();
            Guard.Argument(invoiceHelper, nameof(invoiceHelper)).NotNull();
            Guard.Argument(invoiceNumberRepository, nameof(invoiceNumberRepository)).NotNull();

            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _pricePlanRepository = pricePlanRepository;
            _pendingMembershipChangeRepository = pendingMembershipChangeRepository;
            _paymentService = paymentService;
            _systemDateTime = systemDateTime;
            _invoiceHelper = invoiceHelper;
            _invoiceNumberRepository = invoiceNumberRepository;
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

            paymentIntent ??= await _paymentService.CreatePaymentIntent(user.PaymentCustomerId, pricePlan.Price, cancellationToken);

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

            var nextInvoiceNumberDao = await _invoiceNumberRepository.GetNextInvoiceNumber(cancellationToken);
            var invoiceNumber = _invoiceHelper.CalculateInvoiceNumber(nextInvoiceNumberDao.Number);
            try
            {
                await _invoiceHelper.SendInvoiceAsEmailAttachment(user, paymentIntent, pricePlan.Title, _systemDateTime.GetUtcDateTime(), paymentIntent.Amount, invoiceNumber, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unable to send and invoice for an user {user.Id}. Exception: {ex.Message}");
            }

            var paymentDao = new ProcessedPaymentDao
            {
                UserId = user.Id,
                PaymentServiceId = paymentIntent.Id,
                Amount = paymentIntent.Amount,
                PricePlanId = pricePlan.Id,
                NoOfStamps = pricePlan.NoOfStamps,
                Created = paymentIntent.Created,
                InvoiceNumber = invoiceNumber
            };

            await _paymentRepository.CreatePaymentReceived(paymentDao, cancellationToken);
            _logger.Information($"Previous remaining stamp for the user '{user.Id}' is '{user.RemainingTimeStamps}'.");

            user.CurrentPricePlanId = pricePlan.Id;
            user.PendingPricePlanId = null;
            user.RemainingTimeStamps = pricePlan.NoOfStamps;
            user.PaymentIntentId = paymentIntentId;
            user.MembershipRenewDate = _systemDateTime.GetUtcDateTime().AddMonths(1);
            user.MembershipRenewEpoch = user.MembershipRenewDate.Date.ToEpoch();

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

            var paymentMethod = await _paymentService.GetCustomerPaymentMethod(user.PaymentCustomerId, cancellationToken);

            return paymentMethod.ToResponse();
        }


        public async Task UpdateCustomerPaymentMethod(string userId, string paymentMethodId, AddressDao newAddress, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(paymentMethodId, nameof(paymentMethodId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(newAddress, nameof(newAddress)).NotNull();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            await _paymentService.UpdateCustomerPaymentMethod(paymentMethodId, newAddress, cancellationToken);
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

            var pendingPricePlan = await _pendingMembershipChangeRepository.GetByUser(user.Id, cancellationToken);
            if (pendingPricePlan != null && pendingPricePlan.NewPricePlanId.Equals(pricePlanId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SubscriptionException($"Requested Price plan is in pending state. Please wait for the membership renew date.");
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

                await _pendingMembershipChangeRepository.DeleteByUser(user.Id, cancellationToken);
            }
            else if (user.MembershipRenewDate.Date <= _systemDateTime.GetUtcDateTime().Date && isDowngradedToBasicPlan)
            {
                _logger.Information($"Previous remaining stamp for the user '{user.Id}' is '{user.RemainingTimeStamps}'.");
                user.CurrentPricePlanId = newPricePlan.Id;
                user.PendingPricePlanId = null;
                user.RemainingTimeStamps = newPricePlan.NoOfStamps;
                user.MembershipRenewDate = user.MembershipRenewDate.AddMonths(1);
                user.MembershipRenewEpoch = user.MembershipRenewDate.Date.ToEpoch();

                await _userRepository.UpdateUser(user, cancellationToken);
            }
            else
            {
                user.PendingPricePlanId = newPricePlan.Id;
                await _pendingMembershipChangeRepository.Add(new PendingMembershipChangeDao { Id = Guid.NewGuid().ToString(), UserId = user.Id, RenewEpoch = user.MembershipRenewEpoch, NewPricePlanId = newPricePlan.Id }, cancellationToken);

                await _userRepository.UpdateUser(user, cancellationToken);
            }
        }

        public async Task CancelPendingPricePlan(string userId, string pendingPricePlanId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new SubscriptionException("User does not exists.");

            var pendingPricePlan = await _pendingMembershipChangeRepository.GetByUser(userId, cancellationToken);
            if (pendingPricePlan == null)
            {
                _logger.Warning($"User '{userId}' do not have pending price plan.");
                throw new SubscriptionException($"User '{userId}' do not have pending price plan.");
            }

            if (!pendingPricePlan.NewPricePlanId.Equals(pendingPricePlanId, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Warning($"Pending price plan for the User '{userId}' does not match. Expected: '{pendingPricePlan.NewPricePlanId}' & Actual: '{pendingPricePlanId}'");
                throw new SubscriptionException($"Pending price plan does not match");
            }

            user.PendingPricePlanId = null;
            await _userRepository.UpdateUser(user, cancellationToken);

            await _pendingMembershipChangeRepository.Delete(pendingPricePlan.Id, cancellationToken);
        }
    }
}