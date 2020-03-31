using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
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

        public async Task<PaymentResponseDao> CreatePaymentReceived(PaymentResponseDao paymentResponse, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentResponse, nameof(paymentResponse)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(paymentResponse.RequestId);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, paymentResponse, cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<PaymentResponseDao>(response.Resource.ToString());
        }
    }
}