using System;
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
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri).Where(s => s.Id.ToLower() == userId.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(email);

            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri).Where(s => s.Email.ToLower() == email.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(user);
            AtlasGuard.IsNotNullOrWhiteSpace(user.Email);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, user, new RequestOptions(), false, cancellationToken);

            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }

        public async Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, userId), new RequestOptions { }, cancellationToken);
            }
            catch (DocumentClientException ex)
            {
                throw new UserException($"Unable to delete user '{userId}'.", ex);
            }
        }

        public async Task<UserDao> UpdateUser(UserDao user, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNull(user);

            var response = await Client.UpsertDocumentAsync(_documentCollectionUri, user, cancellationToken: cancellationToken);
            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }
    }
}