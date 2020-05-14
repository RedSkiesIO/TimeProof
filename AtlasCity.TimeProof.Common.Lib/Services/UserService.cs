using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Serilog;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class UserService : IUserService
    {
        private const string Key_File_Name = "timescribe-keystore.txt";
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
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userRepository, nameof(userRepository)).NotNull();
            Guard.Argument(pricePlanRepository, nameof(pricePlanRepository)).NotNull();
            Guard.Argument(paymentService, nameof(paymentService)).NotNull();
            Guard.Argument(emailService, nameof(emailService)).NotNull();
            Guard.Argument(emailTemplateHelper, nameof(emailTemplateHelper)).NotNull();

            _logger = logger;
            _userRepository = userRepository;
            _pricePlanRepository = pricePlanRepository;
            _paymentService = paymentService;
            _emailService = emailService;
            _emailTemplateHelper = emailTemplateHelper;
        }

        [Obsolete]
        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            Guard.Argument(email, nameof(email)).NotNull().NotEmpty().NotWhiteSpace();
            var user = await _userRepository.GetUserByEmail(email, cancellationToken);
            return user;
        }

        public async Task<UserDao> GetUserById(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            var user = await _userRepository.GetUserById(userId, cancellationToken);
            return user;
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            Guard.Argument(user.Email, nameof(user.Email)).NotNull().NotEmpty().NotWhiteSpace();

            var existingUser = await _userRepository.GetUserById(user.Id, cancellationToken);
            if (existingUser != null)
                throw new UserException($"User with identifier '{user.Id}' already exists with an email '{user.Email}'.");

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                _logger.Warning($"When creating user with '{user.Id}', payment user should not exists, but found '{user.PaymentCustomerId}'.");
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
            user.CurrentPricePlanId = freePricePlan.Id;
            user.RemainingTimeStamps = freePricePlan.NoOfStamps;
            user.MembershipStartDate = DateTime.UtcNow;
            user.MembershipRenewDate = DateTime.UtcNow.AddMonths(1);

            var newUser = await _userRepository.CreateUser(user, cancellationToken);
            _logger.Information($"Successfully created user with email '{user.Email}'");

            return newUser;
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

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

        public async Task SendKeyAsEmailAttachment(string userId, string attachmentText, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(attachmentText, nameof(attachmentText)).NotNull().NotEmpty().NotWhiteSpace();

            var user = await _userRepository.GetUserById(userId, cancellationToken);
            if (user == null)
                throw new UserException($"User does not exists '{userId}'. Cannot send the welcome email with key as attachment");

            if (user.KeyEmailSentDate > DateTime.MinValue)
            {
                _logger.Warning($"Key Email already sent for user '{userId}' on '{user.KeyEmailSentDate.ToLongDateTimeString()}'");
                throw new UserException($"Key Email already sent.");
            }

            var emailBody = await _emailTemplateHelper.GetWelcomeEmailBody(user.FirstName, cancellationToken);
            var emailMessage = new EmailDao
            {
                ToAddress = user.Email,
                ToName = user.FullName,
                FromAddress = Constants.AutomatedEmailFromAddress,
                Subject = Constants.WelcomeEmailSubject,
                HtmlBody = emailBody,
            };

            var filePath = await _emailTemplateHelper.CreateFileFromText(attachmentText, Key_File_Name, cancellationToken);
            _logger.Information($"Created key file '{filePath}' for user '{userId}'");

            await _emailService.SendEmail(emailMessage, filePath, cancellationToken);
            _logger.Information($"Sent email with key file as attachment for user '{user.Id}'");

            user.KeyEmailSentDate = DateTime.UtcNow;
            await _userRepository.UpdateUser(user, cancellationToken);

            _emailTemplateHelper.DeleteFileDirectory(filePath);
            _logger.Information($"Deleted key file '{filePath}'");
        }
    }
}