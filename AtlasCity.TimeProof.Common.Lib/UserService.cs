using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Microsoft.Extensions.Logging;

namespace AtlasCity.TimeProof.Common.Lib
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentService _paymentService;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IPaymentService paymentService)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(paymentService, nameof(paymentService)).NotNull();

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
            Guard.Argument(user, nameof(user)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(user.Email);

            UserDao existingPaymentCustomer = null;

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    existingPaymentCustomer = await _paymentService.GetCustomerById(user.PaymentCustomerId, cancellationToken);
                }
                catch (PaymentException ex)
                {
                    user.PaymentCustomerId = string.Empty;
                    _logger.LogWarning(ex, ex.Message);
                }
            }

            // If customer exists in payment service and email does not match, then something is wrong
            if (existingPaymentCustomer != null && !existingPaymentCustomer.Email.Equals(user.Email))
            {
                throw new UserException($"User with payment customer identifier '{user.PaymentCustomerId}' already exiting with mismatch email ids. Expected: '{user.Email}', Actual: '{existingPaymentCustomer.Email}'");
            }

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    _logger.LogInformation($"Creating payment customer in payment service with email '{user.Email}'");
                    user.PaymentCustomerId = await _paymentService.CreatePaymentCustomer(user, cancellationToken);
                }
                catch (PaymentException ex)
                {
                    _logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }

            return await _userRepository.CreateUser(user, cancellationToken);
        }

        public async Task<SetupIntentDao> CreateSetupIntent(string email, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(email);

            var user = await _userRepository.GetUserByEmail(email, cancellationToken);
            if (user == null)
                throw new UserException("Please create the user first.");

            SetupIntentDao setupIntent = null;
            
            if (!string.IsNullOrWhiteSpace(user.SetupIntentId))
            {
                setupIntent = await _paymentService.GetSetupIntent(user.SetupIntentId, cancellationToken);
                if (setupIntent != null)
                    return setupIntent;

                _logger.LogWarning($"Setup intent '{user.SetupIntentId}' is missing from the payment service");
            }

            if (setupIntent == null)
            {
                setupIntent = await _paymentService.CreateSetupIntent(user.PaymentCustomerId, cancellationToken);
                await _userRepository.AddSetupIntent(user.Email, setupIntent.Id, cancellationToken);
            }

            return setupIntent;
        }
    }
}