using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Microsoft.Azure.WebJobs;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AtlasCity.TimeProof.AzureFunction
{
    public class PendingMembershipTrigger
    {
        private readonly ILogger _logger;
        private readonly IPendingMembershipChangeRepository _pendingMembershipChangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPricePlanRepository _pricePlanRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceHelper _invoiceHelper;
        private readonly IInvoiceNumberRepository _invoiceNumberRepository;

        public PendingMembershipTrigger(
            ILogger logger,
            IPendingMembershipChangeRepository pendingMembershipChangeRepository,
            IUserRepository userRepository,
            IPricePlanRepository pricePlanRepository,
            IPaymentRepository paymentRepository,
            IPaymentService paymentService,
            IInvoiceHelper invoiceHelper,
            IInvoiceNumberRepository invoiceNumberRepository
            )
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(pendingMembershipChangeRepository, nameof(pendingMembershipChangeRepository)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(pricePlanRepository, nameof(pricePlanRepository)).NotNull();
            Guard.Argument(paymentRepository, nameof(paymentRepository)).NotNull();
            Guard.Argument(paymentService, nameof(paymentService)).NotNull();
            Guard.Argument(invoiceHelper, nameof(invoiceHelper)).NotNull();
            Guard.Argument(invoiceNumberRepository, nameof(invoiceNumberRepository)).NotNull();

            _logger = logger;
            _pendingMembershipChangeRepository = pendingMembershipChangeRepository;
            _userRepository = userRepository;
            _pricePlanRepository = pricePlanRepository;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _invoiceHelper = invoiceHelper;
            _invoiceNumberRepository = invoiceNumberRepository;
        }

        [FunctionName("PendingMembershipFunction")]
        public async Task Run([TimerTrigger("0 00 6 * * *", RunOnStartup = true)]TimerInfo myTimer, ExecutionContext executionContext)
        {
            await ProcessPendingMemberships(executionContext.FunctionAppDirectory);
            await ProcessRenewingMemberships(executionContext.FunctionAppDirectory);
        }

        private async Task ProcessPendingMemberships(string functionAppDirectory)
        {
            var cancellationToken = new CancellationToken();
            var currentDate = DateTime.UtcNow;
            var pendingMembershipChanges = (await _pendingMembershipChangeRepository.GetByDate(currentDate, cancellationToken)).ToList();
            if (!pendingMembershipChanges.Any())
            {
                _logger.Information($"Unable to find any message at '{currentDate}'.");
                return;
            }

            _logger.Information($"Retrieved '{pendingMembershipChanges.Count()}' pending membership changes at '{currentDate}'.");

            foreach (var pendingChanges in pendingMembershipChanges)
            {
                try
                {
                    await ChangePricePlan(pendingChanges.UserId, pendingChanges.NewPricePlanId, functionAppDirectory, cancellationToken);
                    await _pendingMembershipChangeRepository.DeleteByUser(pendingChanges.UserId, cancellationToken);
                    _logger.Information($"Removed all pending plan update for user '{pendingChanges.UserId}'.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    pendingChanges.Error = ex.Message;
                    try
                    {
                        await _pendingMembershipChangeRepository.Update(pendingChanges, cancellationToken);
                    }
                    catch (Exception exRepo) { _logger.Error(exRepo.Message); }
                }
            }
        }

        private async Task ProcessRenewingMemberships(string functionAppDirectory)
        {
            var cancellationToken = new CancellationToken();
            var currentDate = DateTime.UtcNow;
            var pendingMembershipsToRenew = (await _userRepository.GetRenewalMembershipByDate(currentDate, cancellationToken)).ToList();
            if (!pendingMembershipsToRenew.Any())
            {
                _logger.Information($"There is no pending renew membership on  '{currentDate}'.");
                return;
            }

            _logger.Information($"Found '{pendingMembershipsToRenew.Count()}' pending membership to renew at '{currentDate}'.");

            foreach (var pendingChanges in pendingMembershipsToRenew)
            {
                try
                {
                    await ChangePricePlan(pendingChanges.Id, pendingChanges.CurrentPricePlanId, functionAppDirectory, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
        }

        private async Task ChangePricePlan(string userId, string newPricePlanId, string rootFolder, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newPricePlanId))
                throw new Exception($"Unable to change the membership as either UserId: '{userId}' is missing or NewPricePlan '{newPricePlanId}' is missing");

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new Exception($"User does with '{userId}' identifier does not exists.");

            var newPricePlan = await _pricePlanRepository.GetPricePlanById(newPricePlanId, cancellationToken);
            if (newPricePlan == null)
                throw new Exception($"Unable to find the price plan with '{newPricePlanId}' identifier.");

            if (!newPricePlan.Price.Equals(0))
            {
                var paymentIntent = await _paymentService.CollectPayment(user.PaymentCustomerId, newPricePlan.Price, cancellationToken);
                user.PaymentIntentId = paymentIntent.Id;

                _logger.Information($"Collected payment of '{paymentIntent.Amount}' in minimum unit for user '{user.Id}'.");

                var nextInvoiceNumberDao = await _invoiceNumberRepository.GetNextInvoiceNumber(cancellationToken);
                var invoiceNumber = _invoiceHelper.CalculateInvoiceNumber(nextInvoiceNumberDao.Number);
                try
                {
                    await _invoiceHelper.SendInvoiceAsEmailAttachment(user, paymentIntent, newPricePlan.Title, user.MembershipRenewDate, paymentIntent.Amount, invoiceNumber, rootFolder, cancellationToken);
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
                    PricePlanId = newPricePlan.Id,
                    NoOfStamps = newPricePlan.NoOfStamps,
                    Created = paymentIntent.Created,
                    InvoiceNumber = invoiceNumber
                };

                await _paymentRepository.CreatePaymentReceived(paymentDao, cancellationToken);
            }

            _logger.Information($"Previous remaining stamp for the user '{user.Id}' is '{user.RemainingTimeStamps}'.");

            user.CurrentPricePlanId = newPricePlan.Id;
            user.PendingPricePlanId = null;
            user.RemainingTimeStamps = newPricePlan.NoOfStamps;
            user.MembershipRenewDate = user.MembershipRenewDate.AddMonths(1);
            user.MembershipRenewEpoch = user.MembershipRenewDate.Date.ToEpoch();

            await _userRepository.UpdateUser(user, cancellationToken);
        }
    }
}