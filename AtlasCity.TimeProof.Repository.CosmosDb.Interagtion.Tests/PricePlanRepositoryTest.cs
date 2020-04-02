using System.Configuration;
using System.Linq;
using System.Threading;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtlasCity.TimeProof.Repository.CosmosDb.Interagtion.Tests
{
    [TestClass]
    public class PricePlanRepositoryTest
    {
        IPricePlanRepository _pricePlanRepository;

        [TestInitialize]
        public void Setup()
        {
            // TODO: Sudhir Read from config
            var endpointUrl = "https://dev-time-stamp.documents.azure.com:443/";
            var authorizationKey = "gVpoXnUxYo2eYSPRDeeajoM8Rf3niQDUQQM0PHpDzwMdhzSXeAe7hayZNC4YeyhdnwsJvZjHJc0lZw3jfs0ZTQ==";

            _pricePlanRepository = new PricePlanRepository(endpointUrl, authorizationKey);
        }

        [TestMethod]
        public void Adding_Retrieving_PricePlan_To_And_From_Database()
        {
            var starterPricePlan= new PricePlanDao
            {
                Title = Constants.FreePricePlanTitle,
                Description = "Starter pack with free time stamps",
                Price = 0,
                NoOfStamps = 5
            };

            var basicPricePlan = new PricePlanDao
            {
                Title = "basic",
                Description = "Basic pack with few number of time stamps",
                Price = 599,
                NoOfStamps = 30
            };

            var premiumPricePlan = new PricePlanDao
            {
                Title = "Premium",
                Description = "Premium packs",
                Price = 2599,
                NoOfStamps = 200
            };

            AddPricePlanIfNotExists(starterPricePlan);
            AddPricePlanIfNotExists(basicPricePlan);
            AddPricePlanIfNotExists(premiumPricePlan);

            var pricePlans = _pricePlanRepository.GetPricePlans(CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(pricePlans);
            Assert.IsTrue(pricePlans.Count() >= 3);

            Assert.IsTrue(pricePlans.All(s => !string.IsNullOrWhiteSpace(s.Id)));

            var actualStarterPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(starterPricePlan.Title, System.StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualStarterPricePlan);
            Assert.AreEqual(starterPricePlan.Description, actualStarterPricePlan.Description, true);
            Assert.AreEqual(starterPricePlan.Price, actualStarterPricePlan.Price);
            Assert.AreEqual(starterPricePlan.NoOfStamps, actualStarterPricePlan.NoOfStamps);

            var actualBasicPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(basicPricePlan.Title, System.StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualBasicPricePlan);
            Assert.AreEqual(basicPricePlan.Description, actualBasicPricePlan.Description, true);
            Assert.AreEqual(basicPricePlan.Price, actualBasicPricePlan.Price);
            Assert.AreEqual(basicPricePlan.NoOfStamps, actualBasicPricePlan.NoOfStamps);

            var actualPremiumPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(premiumPricePlan.Title, System.StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualPremiumPricePlan);
            Assert.AreEqual(premiumPricePlan.Description, actualPremiumPricePlan.Description, true);
            Assert.AreEqual(premiumPricePlan.Price, actualPremiumPricePlan.Price);
            Assert.AreEqual(premiumPricePlan.NoOfStamps, actualPremiumPricePlan.NoOfStamps);
        }

        private void AddPricePlanIfNotExists(PricePlanDao pricePlan)
        {
            var existingPricePlan = _pricePlanRepository.GetPricePlanByTitle(pricePlan.Title, CancellationToken.None).GetAwaiter().GetResult();
            if(existingPricePlan == null)
                _pricePlanRepository.AddPricePlans(pricePlan, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
