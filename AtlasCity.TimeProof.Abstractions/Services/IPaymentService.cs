﻿using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.DAO.Payment;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDao> ProcessPayment(PaymentDao payment, string paymentCustomerId, CancellationToken cancellationToken);

        Task<string> CreatePaymentCustomer(UserDao customer, CancellationToken cancellationToken);

        Task<PaymentCustomerDao> GetCustomerById(string paymentCustomerId, CancellationToken cancellationToken);

        Task<PaymentMethodDao> GetCustomerPaymentMethod(string paymentCustomerId, CancellationToken cancellationToken);

        Task DeletePaymentCustomer(string paymentCustomerId, CancellationToken cancellationToken);

        Task<SetupIntentDao> GetSetupIntent(string setupIntentId, CancellationToken cancellationToken);

        Task<SetupIntentDao> CreateSetupIntent(string paymentCustomerId, CancellationToken cancellationToken);
    }
}
