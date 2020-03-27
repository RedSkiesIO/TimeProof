using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentService _paymentService;

        public UserService(ILogger logger, IUserRepository userRepository, IPaymentService paymentService)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(userRepository);
            AtlasGuard.IsNotNull(paymentService);

            _logger = logger;
            _userRepository = userRepository;
            _paymentService = paymentService;
        }

        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(email);

            return await _userRepository.GetUserByEmail(email, cancellationToken);
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(user);
            AtlasGuard.IsNullOrWhiteSpace(user.Email);

            UserDao existingPaymentCustomer = null;

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    existingPaymentCustomer = await _paymentService.GetCustomerById(user.PaymentCustomerId, cancellationToken);
                }
                catch (PaymentServiceException ex)
                {
                    user.PaymentCustomerId = string.Empty;
                    _logger.Warning(ex, ex.Message);
                }
            }

            // If customer exists in payment service and email does not match, then something is wrong
            if (existingPaymentCustomer != null && !existingPaymentCustomer.Email.Equals(user.Email))
            {
                throw new UserException($"User with payment customer identifier '{user.PaymentCustomerId}' already exiting with mismatch email ids. Expected: '{user.Email}', Actual: '{existingPaymentCustomer.Email}'.");
            }

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    _logger.Information($"Creating payment customer in payment service with email '{user.Email}'.");
                    user.PaymentCustomerId = await _paymentService.CreatePaymentCustomer(user, cancellationToken);
                }
                catch (PaymentServiceException ex)
                {
                    _logger.Warning(ex, ex.Message);
                    throw;
                }
            }

            return await _userRepository.CreateUser(user, cancellationToken);
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(userId);

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException("User does not exists.");

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    await _paymentService.DeletePaymentCustomer(user.PaymentCustomerId, cancellationToken);
                }
                catch (PaymentServiceException ex)
                {
                    _logger.Error(ex, $"Unable to delete payment service customer '{user.PaymentCustomerId}'.");
                }
            }

            try
            {
                await _userRepository.DeleteUser(user.Id, cancellationToken);
                _logger.Information($"User '{user.Id}', email '{user.Email}' successfully deleted.'");
            }
            catch (UserException ex)
            {
                _logger.Error(ex, $"Unable to delete user '{user.Id}'.");
            }
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

            var user = await _userRepository.GetUserById(payment.UserId, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
                throw new UserException("PaymentCustomerId is missing. Please create the user first.");

            PaymentResponseDao paymentResponse = null;
            try
            {
                paymentResponse = await _paymentService.ProcessPayment(payment, user.PaymentCustomerId, cancellationToken);
                _logger.Information($"Collected payment for user '{user.Id}'.");

            }
            catch(PaymentServiceException ex)
            {
                _logger.Error(ex, $"Unable to collect payment for user '{user.Id}'.");
                throw ex;
            }

            // TODO: Store the payment in TimeProof repository
            return paymentResponse;
        }
    }
}