using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly CustomerService _customerService;
        private readonly PaymentMethodService _paymentMethodService;
        private readonly SetupIntentService _setupIntentService;

        public StripePaymentService(PaymentIntentService paymentIntentService, CustomerService customerService, PaymentMethodService paymentMethodService, SetupIntentService setupIntentService)
        {
            Guard.Argument(paymentIntentService, nameof(paymentIntentService)).NotNull();
            Guard.Argument(customerService, nameof(customerService)).NotNull();
            Guard.Argument(paymentMethodService, nameof(paymentMethodService)).NotNull();
            Guard.Argument(setupIntentService, nameof(setupIntentService)).NotNull();
            
            _paymentIntentService = paymentIntentService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _setupIntentService = setupIntentService;
        }

        public async Task<string> CreatePaymentCustomer(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            Guard.Argument(user.Email, nameof(user.Email)).NotNull().NotEmpty().NotWhiteSpace();

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
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

            var customer = await _customerService.GetAsync(paymentCustomerId, cancellationToken: cancellationToken);

            if (customer != null)
                return customer.ToPaymentCustomerDao();

            throw new PaymentServiceException($"Unable to find customer with '{paymentCustomerId}' identifier in stripe payment system.");
        }

        public async Task<PaymentMethodDao> GetCustomerPaymentMethod(string paymentCustomerId, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

            var options = new PaymentMethodListOptions
            {
                Customer = paymentCustomerId,
                Type = "card"
            };

            var customerCards = await _paymentMethodService.ListAsync(options: options, cancellationToken: cancellationToken);

            return customerCards.FirstOrDefault().ToPaymentMethodDao();
        }

        public async Task UpdateCustomerPaymentMethod(string paymentMethodId, AddressDao newAddress, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentMethodId, nameof(paymentMethodId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(newAddress, nameof(newAddress)).NotNull();

            var options = new PaymentMethodUpdateOptions
            {
                BillingDetails = new BillingDetailsOptions
                {
                    Address = new AddressOptions
                    {
                        Line1 = newAddress.Line1,
                        Line2 = newAddress.Line2,
                        City = newAddress.City,
                        State = newAddress.State,
                        PostalCode = newAddress.Postcode,
                        Country = newAddress.Country,
                    }
                }
            };

            await _paymentMethodService.UpdateAsync(paymentMethodId, options, cancellationToken: cancellationToken);
        }

        public async Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken)
        {
            Guard.Argument(setupIntentId, nameof(setupIntentId)).NotNull().NotEmpty().NotWhiteSpace();

            var setupIntent = await _setupIntentService.GetAsync(setupIntentId, cancellationToken: cancellationToken);

            if (setupIntent != null)
                return setupIntent.ToSetupIntentDao();

            throw new PaymentServiceException($"Unable to find setup intent with '{setupIntentId}' identifier in stripe payment system.");
        }

        public async Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

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
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

            try
            {
                var deletedCustomer = await _customerService.DeleteAsync(paymentCustomerId, cancellationToken: cancellationToken);
                if (deletedCustomer.Deleted != null && !deletedCustomer.Deleted.Value)
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
            Guard.Argument(paymentIntentId, nameof(paymentIntentId)).NotNull().NotEmpty().NotWhiteSpace();

            var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            return paymentIntent.ToPaymentIntentDao();
        }

        public async Task<PaymentIntentDao> GetPaymentIntentByCustomerId(string paymentCustomerId, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

            var options = new PaymentIntentListOptions
            {
                Customer = paymentCustomerId
            };

            var paymentIntent = await _paymentIntentService.ListAsync(options: options, cancellationToken: cancellationToken);

            return paymentIntent?.FirstOrDefault().ToPaymentIntentDao();
        }

        public async Task<PaymentIntentDao> CreatePaymentIntent(string paymentCustomerId, long amount, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

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
            Guard.Argument(paymentCustomerId, nameof(paymentCustomerId)).NotNull().NotEmpty().NotWhiteSpace();

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
                        throw;
                }
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException($"Unable to take payment for customer '{paymentCustomerId}'", ex);
            }
        }
    }
}