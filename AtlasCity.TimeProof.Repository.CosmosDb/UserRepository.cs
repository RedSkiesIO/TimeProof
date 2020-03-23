using System;
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

        public async Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            Guard.Argument(email, nameof(email)).NotWhiteSpace("email is missing for retrieving an user.");

            var response = Client.CreateDocumentQuery(_documentCollectionUri, $"select * from c where c.email = '{email}'").FirstOrDefault();

            if (response != null)
                return (UserDao)response;

            return null;
        }


        public async Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken)
        {
            Guard.Argument(user, nameof(user)).NotNull();
            Guard.Argument(user.Email, nameof(user.Email)).NotWhiteSpace("User email is missing for creating an user.");

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, user, new RequestOptions(), false, cancellationToken);

            return JsonConvert.DeserializeObject<UserDao>(response.Resource.ToString());
        }
    }
}