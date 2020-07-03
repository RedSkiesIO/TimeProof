using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class KeyRepository : CosmosDbBaseRepository, IKeyRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "Keys";

        private readonly Uri _documentCollectionUri;

        public KeyRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<KeyDao> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<KeyDao>(_documentCollectionUri, option).Where(s => s.UserId.ToLower() == userId.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<KeyDao> CreateKey(KeyDao key, CancellationToken cancellationToken)
        {
            Guard.Argument(key, nameof(key)).NotNull();
            Guard.Argument(key.UserId, nameof(key.UserId)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(key.KeyDetails, nameof(key.KeyDetails)).NotNull().NotEmpty().NotWhiteSpace();

            var existingUserKey = await GetByUserId(key.UserId, cancellationToken);
            if (existingUserKey != null)
            {
               throw new UserException($"Key already created for the user {key.UserId}");
            }

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, key, new RequestOptions(), false, cancellationToken);
            return JsonConvert.DeserializeObject<KeyDao>(response.Resource.ToString());
        }
    }
}