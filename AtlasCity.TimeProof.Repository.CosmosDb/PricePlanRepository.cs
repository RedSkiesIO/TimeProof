using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Dawn;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Repository.CosmosDb
{
    public class PricePlanRepository : CosmosDbBaseRepository, IPricePlanRepository
    {
        private const string DatabaseId = "DocumentStamp";
        private const string CollectionId = "PricePlans";

        private readonly Uri _documentCollectionUri;

        public PricePlanRepository(string endpointUrl, string authorizationKey) :
          base(endpointUrl, authorizationKey)
        {
            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        }

        public async Task<IEnumerable<PricePlanDao>> GetPricePlans(CancellationToken cancellationToken)
        {
            var response = Client.CreateDocumentQuery(_documentCollectionUri, $"select * from c").AsEnumerable();

            if (response != null)
                return response.Select(s => (PricePlanDao)s).AsEnumerable();

            return null;
        }

        public async Task<PricePlanDao> GetPricePlanByTitle(string pricePlanTitle, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNullOrWhiteSpace(pricePlanTitle);

            var response = Client.CreateDocumentQuery<PricePlanDao>(_documentCollectionUri).Where(s => s.Title.ToLower() == pricePlanTitle.ToLower()).AsEnumerable().FirstOrDefault();

            if (response != null)
                return response;

            return null;
        }

        public async Task<PricePlanDao> AddPricePlans(PricePlanDao pricePlan, CancellationToken cancellationToken)
        {
            Guard.Argument(pricePlan, nameof(pricePlan)).NotNull();
            AtlasGuard.IsNullOrWhiteSpace(pricePlan.Title);

            var response = await Client.CreateDocumentAsync(_documentCollectionUri, pricePlan, new RequestOptions(), false, cancellationToken);

            return JsonConvert.DeserializeObject<PricePlanDao>(response.Resource.ToString());
        }
    }
}