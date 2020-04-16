using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class PaymentRepository : CosmosDbBaseRepository, IPaymentRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "Payments";

        private readonly Uri _documentCollectionUri;

        public PaymentRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<ProcessedPaymentDao> CreatePaymentReceived(ProcessedPaymentDao payment, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(payment);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, payment, cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<ProcessedPaymentDao>(response.Resource.ToString());
        }

        public async  Task<ProcessedPaymentDao> GetLastPayment(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var response = Client.CreateDocumentQuery<ProcessedPaymentDao>(_documentCollectionUri).Where(s => s.UserId.ToLower() == userId.ToLower()).OrderByDescending(s => s.Created).AsEnumerable().FirstOrDefault();
            return response;
        }
    }
}