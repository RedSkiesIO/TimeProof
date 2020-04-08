using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;
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
        private readonly PaymentMethodService _paymentMethodServiceService;
        private readonly SetupIntentService _setupIntentService;

        public StripePaymentService(PaymentIntentService paymentIntentService, CustomerService customerService, PaymentMethodService paymentMethodServiceService, SetupIntentService setupIntentService)
        {
            AtlasGuard.IsNotNull(paymentIntentService);
            AtlasGuard.IsNotNull(customerService);
            AtlasGuard.IsNotNull(paymentMethodServiceService);
            AtlasGuard.IsNotNull(setupIntentService);

            _paymentIntentService = paymentIntentService;
            _customerService = customerService;
            _paymentMethodServiceService = paymentMethodServiceService;
            _setupIntentService = setupIntentService;
        }

        public async Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, string paymentCustomerId, string setupIntentId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(payment);
            AtlasGuard.IsNotNullOrWhiteSpace(payment.UserId);
            AtlasGuard.IsNotNullOrWhiteSpace(payment.Email);
            AtlasGuard.IsNotNullOrWhiteSpace(payment.PaymentMethodId);
            AtlasGuard.IsNotNullOrWhiteSpace(paymentCustomerId);
            AtlasGuard.IsNotNullOrWhiteSpace(setupIntentId);

            var options = new PaymentIntentCreateOptions
            {
                Amount = payment.Amount,
                Currency = "gbp",
                Customer = paymentCustomerId,
                PaymentMethod = payment.PaymentMethodId,
                ReceiptEmail = payment.Email,
                Confirm = true,
                OffSession = true
            };

            try
            {
                var response = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);
                if (response.StripeResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new PaymentServiceException($"Unable to take payment for user '{payment.UserId}', payment method '{payment.PaymentMethodId}' in stripe payment system. The status is '{response.StripeResponse.StatusCode}' and content '{response.StripeResponse.Content}'.");
                }

                await _setupIntentService.ConfirmAsync(setupIntentId, cancellationToken: cancellationToken);

                return response.StripeResponse.ToStripeResponseDao();
            }
            catch (StripeException ex)
            {
                throw new PaymentServiceException($"Unable to take payment for user '{payment.UserId}', payment method '{payment.PaymentMethodId}' in stripe payment system.", ex);
            }
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

            var customerCards = await _paymentMethodServiceService.ListAsync(options: options, cancellationToken: cancellationToken);

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
    }
}