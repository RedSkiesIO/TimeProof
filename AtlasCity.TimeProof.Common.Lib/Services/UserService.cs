using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPricePlanRepository _pricePlanRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateHelper _emailTemplateHelper;

        public UserService(
            ILogger logger,
            IUserRepository userRepository,
            IPricePlanRepository pricePlanRepository,
            IPaymentService paymentService,
            IEmailService emailService,
            IEmailTemplateHelper emailTemplateHelper)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(userRepository);
            AtlasGuard.IsNotNull(pricePlanRepository);
            AtlasGuard.IsNotNull(paymentService);
            AtlasGuard.IsNotNull(emailService);
            AtlasGuard.IsNotNull(emailTemplateHelper);

            _logger = logger;
            _userRepository = userRepository;
            _pricePlanRepository = pricePlanRepository;
            _paymentService = paymentService;
            _emailService = emailService;
            _emailTemplateHelper = emailTemplateHelper;
        }

        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(email);

            var user =  await _userRepository.GetUserByEmail(email, cancellationToken);
            return user;
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(user);
            AtlasGuard.IsNotNullOrWhiteSpace(user.Email);

            var existingUser = await _userRepository.GetUserByEmail(user.Email, cancellationToken);
            if (existingUser != null)
                throw new UserException($"User with email '{user.Email}' already exists.");

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                _logger.Warning($"When creating user with email '{user.Email}', payment user should not exists, but found '{user.PaymentCustomerId}'.");
                user.PaymentCustomerId = string.Empty;
            }

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

            var freePricePlan = await _pricePlanRepository.GetPricePlanByTitle(Constants.FreePricePlanTitle, cancellationToken);
            user.PricePlanId = freePricePlan.Id;

            var newUser = await _userRepository.CreateUser(user, cancellationToken);
            _logger.Information($"Successfully created user with email '{user.Email}'");

            var emailBody = await _emailTemplateHelper.GetWelcomeEmailBody(user.FullName, cancellationToken);
            var emailMessage = new EmailDao
            {
                ToAddress = user.Email,
                ToName = user.FullName,
                FromAddress = Constants.AutomatedEmailFromAddress,
                Subject = Constants.WelcomeEmailSubject,
                HtmlBody = emailBody
            };

            await _emailService.SendEmail(emailMessage, cancellationToken);
            return newUser;
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

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
    }
}