using System;
using Dawn;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public abstract class CosmosDbBaseRepository
    {
        public DocumentClient Client { get; private set; }

        public CosmosDbBaseRepository(string endpointUrl, string authorizationKey)
        {
          
            Guard.Argument(endpointUrl, nameof(endpointUrl)).NotWhiteSpace("Missing endpointUrl");
            Guard.Argument(authorizationKey, nameof(authorizationKey)).NotWhiteSpace("Missing authorizationKey");

            var client = new DocumentClient(new Uri(endpointUrl), authorizationKey,
               new JsonSerializerSettings
               {
                   NullValueHandling = NullValueHandling.Ignore
               });

            Client = client;
        }
    }
}
