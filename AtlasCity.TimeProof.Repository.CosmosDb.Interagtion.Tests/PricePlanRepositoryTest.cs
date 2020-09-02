using System;
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
            var basicPricePlan= new PricePlanDao
            {
                Title = Constants.FreePricePlanTitle,
                Description = "Slow transaction",
                Price = 0,
                NoOfStamps = 2,
                TransactionPrice = 1,
                GasPrice = 2,
                PaymentFrquencyDescription = "Per month"
            };

            var standardPricePlan = new PricePlanDao
            {
                Title = "Startups",
                Description = "Fast transaction",
                Price = 799,
                NoOfStamps = 10,
                TransactionPrice = 5,
                GasPrice = 10,
                PaymentFrquencyDescription = "Per month, billed monthly"
            };

            var premiumPricePlan = new PricePlanDao
            {
                Title = "Content Creators",
                Description = "Fast transaction",
                Price = 2399,
                NoOfStamps = 30,
                TransactionPrice = 5,
                GasPrice = 10,
                PaymentFrquencyDescription = "Per month, billed monthly"
            };

            var powerUserPricePlan = new PricePlanDao
            {
                Title = "Power Users",
                Description = "Fast transaction",
                Price = 4499,
                NoOfStamps = 60,
                TransactionPrice = 5,
                GasPrice = 10,
                PaymentFrquencyDescription = "Per month, billed monthly"
            };


            AddPricePlanIfNotExists(basicPricePlan);
            AddPricePlanIfNotExists(standardPricePlan);
            AddPricePlanIfNotExists(premiumPricePlan);
            AddPricePlanIfNotExists(powerUserPricePlan);

            var pricePlans = _pricePlanRepository.GetPricePlans(CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(pricePlans);
            Assert.IsTrue(pricePlans.Count() >= 3);

            Assert.IsTrue(pricePlans.All(s => !string.IsNullOrWhiteSpace(s.Id)));

            var actualBasicPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(basicPricePlan.Title, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualBasicPricePlan);
            Assert.AreEqual(basicPricePlan.Description, actualBasicPricePlan.Description, true);
            Assert.AreEqual(basicPricePlan.Price, actualBasicPricePlan.Price);
            Assert.AreEqual(basicPricePlan.NoOfStamps, actualBasicPricePlan.NoOfStamps);
            Assert.AreEqual(basicPricePlan.PaymentFrquencyDescription, basicPricePlan.PaymentFrquencyDescription);

            var actualStandardPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(standardPricePlan.Title, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualStandardPricePlan);
            Assert.AreEqual(standardPricePlan.Description, actualStandardPricePlan.Description, true);
            Assert.AreEqual(standardPricePlan.Price, actualStandardPricePlan.Price);
            Assert.AreEqual(standardPricePlan.NoOfStamps, actualStandardPricePlan.NoOfStamps);
            Assert.AreEqual(standardPricePlan.PaymentFrquencyDescription, standardPricePlan.PaymentFrquencyDescription);

            var actualPremiumPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(premiumPricePlan.Title, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualPremiumPricePlan);
            Assert.AreEqual(premiumPricePlan.Description, actualPremiumPricePlan.Description, true);
            Assert.AreEqual(premiumPricePlan.Price, actualPremiumPricePlan.Price);
            Assert.AreEqual(premiumPricePlan.NoOfStamps, actualPremiumPricePlan.NoOfStamps);

            var actualPowerUserPricePlan = pricePlans.FirstOrDefault(s => s.Title.Equals(powerUserPricePlan.Title, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(actualPremiumPricePlan);
            Assert.AreEqual(powerUserPricePlan.Description, actualPowerUserPricePlan.Description, true);
            Assert.AreEqual(powerUserPricePlan.Price, actualPowerUserPricePlan.Price);
            Assert.AreEqual(powerUserPricePlan.NoOfStamps, actualPowerUserPricePlan.NoOfStamps);
            Assert.AreEqual(powerUserPricePlan.PaymentFrquencyDescription, actualPowerUserPricePlan.PaymentFrquencyDescription);
            Assert.AreEqual(powerUserPricePlan.PaymentFrquencyDescription, actualPowerUserPricePlan.PaymentFrquencyDescription);
        }

        private void AddPricePlanIfNotExists(PricePlanDao pricePlan)
        {
            var existingPricePlan = _pricePlanRepository.GetPricePlanByTitle(pricePlan.Title, CancellationToken.None).GetAwaiter().GetResult();
            if(existingPricePlan == null)
                _pricePlanRepository.AddPricePlans(pricePlan, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
