using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Dawn;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class TimestampRepository : CosmosDbBaseRepository, ITimestampRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "Timestamps";

        private readonly Uri _documentCollectionUri;

        public TimestampRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<IEnumerable<TimestampDao>> GetTimestampByUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotWhiteSpace("User identifier is missing for retrieving timestamps.");

            var response = Client.CreateDocumentQuery(_documentCollectionUri, $"select * from c where c.user = '{userId}'").AsEnumerable();

            return response.Select(s => (TimestampDao)s).AsEnumerable();
        }

        public async Task<TimestampDao> CreateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            Guard.Argument(timestamp, nameof(timestamp)).NotNull();

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, timestamp, new RequestOptions(), false, cancellationToken);

            var newTimestamp = JsonConvert.DeserializeObject<TimestampDao>(response.Resource.ToString());
            return newTimestamp;
        }

        public async Task<TimestampDao> UpdateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            Guard.Argument(timestamp, nameof(timestamp)).NotNull();
            Guard.Argument(timestamp.Id, nameof(timestamp.Id)).NotWhiteSpace("Timestamp identifier is missing for an update.");

            throw new NotImplementedException();
        }
    }
}
