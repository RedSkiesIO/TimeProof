using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class InvoiceNumberRepository : CosmosDbBaseRepository, IInvoiceNumberRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "InvoiceNumber";

        private readonly Uri _documentCollectionUri;

        public InvoiceNumberRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<InvoiceNumberDao> GetNextInvoiceNumber(CancellationToken cancellationToken)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<InvoiceNumberDao>(_documentCollectionUri, option).AsEnumerable().FirstOrDefault();

            response ??= new InvoiceNumberDao {Id = Guid.NewGuid().ToString(), Number = 0};

            // Maximum cycle reached. Reset
            if (response.Number > Constants.MaxInvoiceNumberCycle)
            {
                response.Number = 0;
            }

            response.Number++;

            var upsertResponse = await Client.UpsertDocumentAsync(_documentCollectionUri, response, cancellationToken: cancellationToken);
            var newNumber =  JsonConvert.DeserializeObject<InvoiceNumberDao>(upsertResponse.Resource.ToString());

            return newNumber;
        }
    }
}