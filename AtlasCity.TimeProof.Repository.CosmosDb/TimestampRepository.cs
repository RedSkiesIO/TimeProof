using System;
using System.Collections.Generic;
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
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var response = Client.CreateDocumentQuery<TimestampDao>(_documentCollectionUri).Where(s => s.UserId.ToLower() == userId.ToLower()).AsEnumerable();
            return response;
        }

        public async Task<int> GetTimestampCountByUser(string userId, DateTime fromDateTime, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);
            AtlasGuard.IsNotNull(fromDateTime);

            var responseCount = Client.CreateDocumentQuery<TimestampDao>(_documentCollectionUri).Where(s => s.UserId.ToLower() == userId.ToLower() && s.Timestamp >= fromDateTime).Count();
            return responseCount;
        }

        public async Task<TimestampDao> CreateTimestamp(TimestampDao timestamp, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(timestamp);

            timestamp.Timestamp = DateTime.UtcNow;
            var response = await Client.CreateDocumentAsync(_documentCollectionUri, timestamp, new RequestOptions(), false, cancellationToken);

            var newTimestamp = JsonConvert.DeserializeObject<TimestampDao>(response.Resource.ToString());
            return newTimestamp;
        }
    }
}
