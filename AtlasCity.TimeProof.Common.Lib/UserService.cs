using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            Guard.Argument(email, nameof(email)).NotWhiteSpace("email is missing for retrieving an user.");

            return await _userRepository.GetUserByEmail(email, cancellationToken);
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            Guard.Argument(user.Email, nameof(user.Email)).NotWhiteSpace("User email is missing for creating an user.");

            if (!string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
                    await _paymentService.GetCustomerById(user.PaymentCustomerId, cancellationToken);
                }
                catch (PaymentException ex)
                {
                    user.PaymentCustomerId = string.Empty;
                    _logger.LogWarning(ex, ex.Message);
                }
            }

            if (string.IsNullOrWhiteSpace(user.PaymentCustomerId))
            {
                try
                {
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
    }
}