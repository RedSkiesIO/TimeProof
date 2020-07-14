using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IInvoiceHelper
    {
        public string CalculateInvoiceNumber(long number);

        Task SendInvoiceAsEmailAttachment(UserDao user, PaymentIntentDao paymentIntent, string subscriptionName, DateTime subscriptionStartDate, long totalAmount, string invoiceNumber, string rootFolder, CancellationToken cancellationToken);
    }
}
