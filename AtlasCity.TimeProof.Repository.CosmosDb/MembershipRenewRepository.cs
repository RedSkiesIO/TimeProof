using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class MembershipRenewRepository : CosmosDbBaseRepository, IMembershipRenewRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "PendingRenews";

        private readonly Uri _documentCollectionUri;

        public MembershipRenewRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<MembershipRenewDao> AddMembershipRenew(MembershipRenewDao membershipRenew, CancellationToken cancellationToken)
        {
            Guard.Argument(membershipRenew, nameof(membershipRenew)).NotNull();

            await DeleteMembershipRenewByUser(membershipRenew.UserId, cancellationToken);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, membershipRenew, cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<MembershipRenewDao>(response.Resource.ToString());
        }

        public async Task<IEnumerable<MembershipRenewDao>> GetMembershipRenewByDate(DateTime renewDate, CancellationToken cancellationToken)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<MembershipRenewDao>(_documentCollectionUri, option).Where(s => s.MembershipRenewDate.Date <= renewDate.Date && string.IsNullOrEmpty(s.Error)).AsEnumerable();
            return response;
        }

        public async Task DeleteMembershipRenew(string id, CancellationToken cancellationToken)
        {
            Guard.Argument(id, nameof(id)).NotNull().NotEmpty().NotWhiteSpace();

            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { }, cancellationToken);
            }
            catch (DocumentClientException ex)
            {
                throw new MembershipRenewException($"Unable to delete the renew messages '{id}'.", ex);
            }
        }

        private async Task DeleteMembershipRenewByUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var pendingRenewsList = (await GetMembershipRenewByUser(userId, cancellationToken)).ToList();

            if (!pendingRenewsList.Any())
                return;

            try
            {
                pendingRenewsList.ForEach(async s => await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, s.Id), new RequestOptions { }, cancellationToken));
            }
            catch (DocumentClientException ex)
            {
                throw new MembershipRenewException($"Unable to delete the renew messages '{string.Join(",", pendingRenewsList.Select(s => s.Id))}' for user '{userId}'.", ex);
            }
        }

        private async Task<IEnumerable<MembershipRenewDao>> GetMembershipRenewByUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<MembershipRenewDao>(_documentCollectionUri, option).Where(s => s.UserId.ToLower() == userId.ToLower()).AsEnumerable();
            return response;
        }
    }
}