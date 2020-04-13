using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Stripe;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly CustomerService _customerService;
        private readonly PaymentMethodService _paymentMethodService;
        private readonly SetupIntentService _setupIntentService;

        public PaymentMethodService PaymentMethodService => _paymentMethodService;

        public StripePaymentService(PaymentIntentService paymentIntentService, CustomerService customerService, PaymentMethodService paymentMethodService, SetupIntentService setupIntentService)
        {
            AtlasGuard.IsNotNull(paymentIntentService);
            AtlasGuard.IsNotNull(customerService);
            AtlasGuard.IsNotNull(paymentMethodService);
            AtlasGuard.IsNotNull(setupIntentService);

            _paymentIntentService = paymentIntentService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _setupIntentService = setupIntentService;
        }

        public async Task<string> CreatePaymentCustomer(UserDao user, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(user);
            AtlasGuard.IsNotNullOrWhiteSpace(user.Email);

            var options = new CustomerCreateOptions
            {
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            };

            var newStripeCustomer = await _customerService.CreateAsync(options, cancellationToken: cancellationToken);
            if (newStripeCustomer != null)
                return newStripeCustomer.Id;

            throw new PaymentServiceException($"Unable to create customer with email '{options.Email}' in stripe payment system.");
        }

        public async Task<PaymentCustomerDao> GetCustomerById(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            var customer = await _customerService.GetAsync(paymentCustomerId, cancellationToken: cancellationToken);

            if (customer != null)
                return customer.ToPaymentCustomerDao();

            throw new PaymentServiceException($"Unable to find customer with '{paymentCustomerId}' identifier in stripe payment system.");
        }


        public async Task<PaymentMethodDao> GetCustomerPaymentMethod(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            var options = new PaymentMethodListOptions
            {
                Customer = paymentCustomerId,
                Type = "card"
            };

            var customerCards = await _paymentMethodService.ListAsync(options: options, cancellationToken: cancellationToken);

            return customerCards.FirstOrDefault().ToPaymentMethodDao();
        }

        public async Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(setupIntentId);

            var setupIntent = await _setupIntentService.GetAsync(setupIntentId, cancellationToken: cancellationToken);

            if (setupIntent != null)
                return setupIntent.ToSetupIntentDao();

            throw new PaymentServiceException($"Unable to find setup intent with '{setupIntentId}' identifier in stripe payment system.");
        }

        public async Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            var options = new SetupIntentCreateOptions
            {
                Customer = paymentCustomerId,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var newSetupIntent = await _setupIntentService.CreateAsync(options, cancellationToken: cancellationToken);

            if (newSetupIntent != null)
                return newSetupIntent.ToSetupIntentDao();

            throw new PaymentServiceException($"Unable to create customer with customer identifier '{paymentCustomerId}' in stripe payment system.");
        }

        public async Task DeletePaymentCustomer(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            try
            {
                var deletedCustomer = await _customerService.DeleteAsync(paymentCustomerId, cancellationToken: cancellationToken);
                if (!deletedCustomer.Deleted.Value)
                {
                    throw new PaymentServiceException($"Unable to delete user '{paymentCustomerId}' from stripe payment service.");
                }
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException(ex);
            }
        }

        public async Task<PaymentIntentDao> GetPaymentIntent(string paymentIntentId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentIntentId);

            var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            return paymentIntent.ToPaymentIntentDao();
        }

        public async Task<PaymentIntentDao> GetPaymentIntentByCustomerId(string paymentCustomerId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            var options = new PaymentIntentListOptions
            {
                Customer = paymentCustomerId
            };

            var paymentIntent = await _paymentIntentService.ListAsync(options: options, cancellationToken: cancellationToken);

            if (paymentIntent != null)
                return paymentIntent.FirstOrDefault().ToPaymentIntentDao();

            return null;
        }

        public async Task<PaymentIntentDao> CreatePaymentIntent(string paymentCustomerId, long amount, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Customer = paymentCustomerId,
                    Amount = amount,
                    Currency = "gbp",

                    SetupFutureUsage = "on_session"
                };

                var paymentIntent = await _paymentIntentService.CreateAsync(options: options, cancellationToken: cancellationToken);

                return paymentIntent.ToPaymentIntentDao();
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException($"Unable to create payment intent for customer '{paymentCustomerId}'", ex);
            }
        }

        public async Task<PaymentIntentDao> CollectPayment(string paymentCustomerId, long amount, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);

            try
            {
                var existingPaymentMethod = await GetCustomerPaymentMethod(paymentCustomerId, cancellationToken);

                if (existingPaymentMethod == null)
                    throw new PaymentServiceException($" Cannot collect the payment as payment method does not exists for customer '{paymentCustomerId}'.");

                var options = new PaymentIntentCreateOptions
                {
                    Customer = paymentCustomerId,
                    Amount = amount,
                    Currency = "gbp",

                    PaymentMethod = existingPaymentMethod.Id,
                    Confirm = true,
                    OffSession = true,
                };

                var paymentIntent = await _paymentIntentService.CreateAsync(options: options, cancellationToken: cancellationToken);

                return paymentIntent.ToPaymentIntentDao();
            }
            catch (StripeException ex)
            {
                switch (ex.StripeError.ErrorType)
                {
                    case "card_error":
                        // Error code will be authentication_required if authentication is needed
                        throw new PaymentServiceException($"Unable to take payment customer '{paymentCustomerId}'. Error code: {ex.StripeError.Code}, PaymentIntentId '{ex.StripeError.PaymentIntent.Id}'", ex);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException($"Unable to take payment for customer '{paymentCustomerId}'", ex);
            }
        }
    }
}