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
    public class UserRepository : CosmosDbBaseRepository, IUserRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "Users";

        private readonly Uri _documentCollectionUri;

        public UserRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<UserDao> GetUserById(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri, option).Where(s => s.Id.ToLower() == userId.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            Guard.Argument(user.Id, nameof(user.Id)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(user.Email, nameof(user.Email)).NotNull().NotEmpty().NotWhiteSpace();

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, user, new RequestOptions(), false, cancellationToken);

            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            Guard.Argument(userId, nameof(userId)).NotNull().NotEmpty().NotWhiteSpace();

            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, userId), new RequestOptions { PartitionKey = new PartitionKey(userId) }, cancellationToken);
            }
            catch (DocumentClientException ex)
            {
                throw new UserException($"Unable to delete user '{userId}'.", ex);
            }
        }

        public async Task<UserDao> UpdateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();

            var response = await Client.UpsertDocumentAsync(_documentCollectionUri, user, cancellationToken: cancellationToken);
            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }


        public async Task<IEnumerable<UserDao>> GetRenewalMembershipByDate(DateTime renewDate, CancellationToken cancellationToken)
        {
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri, option).Where(s => s.MembershipRenewEpoch <= renewDate.Date.ToEpoch()).AsEnumerable();
            return response;
        }
    }
}