using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
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
            AtlasGuard.IsNullOrWhiteSpace(userId);

            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri).Where(s => s.Id.ToLower() == userId.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(email);

            var response = Client.CreateDocumentQuery<UserDao>(_documentCollectionUri).Where(s => s.Email.ToLower() == email.ToLower()).AsEnumerable().FirstOrDefault();
            return response;
        }

        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(user.Email);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, user, new RequestOptions(), false, cancellationToken);

            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }

        public async Task AddSetupIntent(string email, string setupIntentId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(email);
            AtlasGuard.IsNullOrWhiteSpace(setupIntentId);

            var user = await GetUserByEmail(email, cancellationToken);
            if (user == null)
                throw new UserException($"User with email '{email}' does not exists. Please create the user first");

            user.SetupIntentId = setupIntentId;

            await Client.UpsertDocumentAsync(_documentCollectionUri, user, null, false, cancellationToken);
        }
    }
}