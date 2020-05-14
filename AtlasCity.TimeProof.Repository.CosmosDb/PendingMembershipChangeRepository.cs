using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class PendingMembershipChangeRepository : CosmosDbBaseRepository, IPendingMembershipChangeRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "PendingRenews";

        private readonly Uri _documentCollectionUri;

        public PendingMembershipChangeRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<PendingMembershipChangeDao> Add(PendingMembershipChangeDao membershipRenew, CancellationToken cancellationToken)
        {
            Guard.Argument(membershipRenew, nameof(membershipRenew)).NotNull();

            await DeleteByUser(membershipRenew.UserId, cancellationToken);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, membershipRenew, cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<PendingMembershipChangeDao>(response.Resource.ToString());
        }

        public async Task<IEnumerable<PendingMembershipChangeDao>> GetByDate(DateTime renewDate, CancellationToken cancellationToken)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<PendingMembershipChangeDao>(_documentCollectionUri, option).Where(s => s.RenewEpoch <= renewDate.Date.ToEpoch() && (s.Error ?? string.Empty) == string.Empty).AsEnumerable();
            return response;
        }

        public async Task<PendingMembershipChangeDao> GetByUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<PendingMembershipChangeDao>(_documentCollectionUri, option).Where(s => s.UserId.ToLower() == userId.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task Delete(string id, CancellationToken cancellationToken)
        {
            Guard.Argument(id, nameof(id)).NotNull().NotEmpty().NotWhiteSpace();

            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(id) }, cancellationToken);
            }
            catch (DocumentClientException ex)
            {
                throw new MembershipRenewException($"Unable to delete the renew messages '{id}'.", ex);
            }
        }

        public async Task Update(PendingMembershipChangeDao pendingMembershipChange, CancellationToken cancellationToken)
        {
            Guard.Argument(pendingMembershipChange, nameof(pendingMembershipChange)).NotNull();

            await Client.UpsertDocumentAsync(_documentCollectionUri, pendingMembershipChange, cancellationToken: cancellationToken);
        }

        public async Task DeleteByUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var pendingRenewsList = Client.CreateDocumentQuery<PendingMembershipChangeDao>(_documentCollectionUri, option).Where(s => s.UserId.ToLower() == userId.ToLower()).AsEnumerable();

            if (!pendingRenewsList.Any())
                return;

            try
            {
                pendingRenewsList.ToList().ForEach(async s => await Delete(s.Id, cancellationToken));
            }
            catch (DocumentClientException ex)
            {
                throw new MembershipRenewException($"Unable to delete the renew messages '{string.Join(",", pendingRenewsList.Select(s => s.Id))}' for user '{userId}'.", ex);
            }
        }
    }
}