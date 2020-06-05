using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Unit.Tests.Helpers
{
    [TestClass]
    public class EthHelperTest
    {
        IEthHelper etheHelper;
        Mock<IEthClient> ethClientMock;

        [TestInitialize]
        public void Setup()
        {
            ethClientMock = new Mock<IEthClient>();
            
            etheHelper = new EthHelper(new EthSettings { }, ethClientMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EthHelper_With_Null_EthSettings_Should_Throw_An_Exception()
        {
            new EthHelper(null, ethClientMock.Object);
        }

        [TestMethod]
        public void Calling_GetGasPrice_HappyPath()
        {
            ethClientMock.Setup(s => s.GetCryptoCurrencyValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(164.6));

            var actualResult = etheHelper.GetGasPrice(2.5, default).GetAwaiter().GetResult();
            Assert.AreEqual(5, actualResult);
        }
    }
}
