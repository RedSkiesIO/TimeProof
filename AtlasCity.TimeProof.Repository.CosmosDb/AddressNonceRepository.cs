using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Dawn;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class AddressNonceRepository : CosmosDbBaseRepository, IAddressNonceRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "TimescribeNonce";

        private readonly Uri _documentCollectionUri;

        public AddressNonceRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<NonceDao> GetNonceByAddress(string address, CancellationToken cancellationToken)
        {
            Guard.Argument(address, nameof(address)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<NonceDao>(_documentCollectionUri, option).Where(s => s.Address.ToLower() == address.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<NonceDao> UpdateAddressNonce(NonceDao nonce, CancellationToken cancellationToken)
        {
            Guard.Argument(nonce, nameof(nonce)).NotNull();

            var response = await Client.UpsertDocumentAsync(_documentCollectionUri, nonce, cancellationToken: cancellationToken);

            var updatedTimestamp = JsonConvert.DeserializeObject<NonceDao>(response.Resource.ToString());
            return updatedTimestamp;
        }
    }
}
