using System;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
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

        public async Task<PaymentIntentDao> CreatePaymentReceived(PaymentIntentDao paymentIntent, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(paymentIntent);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, paymentIntent, cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<PaymentIntentDao>(response.Resource.ToString());
        }
    }
}