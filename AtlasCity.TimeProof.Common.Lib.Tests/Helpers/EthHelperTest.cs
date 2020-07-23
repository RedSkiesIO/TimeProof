using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.EthResponse;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Tests.Helpers
{
    [TestClass]
    public class EthHelperTest
    {
        IEthHelper etheHelper;
        Mock<IEthClient> ethClientMock;
        Mock<ILogger> loggerMock;

        [TestInitialize]
        public void Setup()
        {
            ethClientMock = new Mock<IEthClient>();
            loggerMock = new Mock<ILogger>();

            etheHelper = new EthHelper(new EthSettings { }, ethClientMock.Object, loggerMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initilising_EthHelper_With_Null_EthSettings_Should_Throw_An_Exception()
        {
            new EthHelper(null, ethClientMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void Calling_GetGasPrice_HappyPath()
        {
            ethClientMock.Setup(s => s.GetCryptoCurrencyValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(164.6));

            var actualResult = etheHelper.GetGasPrice(2.5, default).GetAwaiter().GetResult();
            Assert.AreEqual(5, actualResult);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetFreePlanGwei_With_Null_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetFreePlanGwei(null, default).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetFreePlanGwei_With_Empty_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetFreePlanGwei(string.Empty, default).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetFreePlanGwei_With_WhiteSpace_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetFreePlanGwei(" ", default).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_SafeLow_Below_70Percent_Of_Average_As_of_100_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "100", AverageWait = "4", SafeLow = "69", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(Math.Ceiling(ethGasStationPrice.AverageGwei*.7), actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.AverageWait.AsInt(), 0), actualResult.WaitTime);
            loggerMock.Verify(s => s.Information($"FastGwei: '{ethGasStationPrice.FastGwei}', AverageGwei: '{ethGasStationPrice.AverageGwei}', SafeLowGwei: '{ethGasStationPrice.SafeLowGwei}'"), Times.Once);
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_SafeLow_Exact_70Percent_Of_Average_As_of_100_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "100", AverageWait = "4", SafeLow = "70", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(ethGasStationPrice.SafeLowGwei, actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.SafeLowWait.AsInt(), 0), actualResult.WaitTime);
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_SafeLow_Above_70Percent_Of_Average_As_of_100_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "100", AverageWait = "4", SafeLow = "71", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(ethGasStationPrice.SafeLowGwei, actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.SafeLowWait.AsInt(), 0), actualResult.WaitTime);
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_SafeLow_Below_70Percent_Of_Average_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "89", AverageWait = "4", SafeLow = "62", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(Math.Ceiling(ethGasStationPrice.AverageGwei * .7), actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.AverageWait.AsInt(), 0), actualResult.WaitTime);
            loggerMock.Verify(s => s.Information($"FastGwei: '{ethGasStationPrice.FastGwei}', AverageGwei: '{ethGasStationPrice.AverageGwei}', SafeLowGwei: '{ethGasStationPrice.SafeLowGwei}'"), Times.Once);
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_SafeLow_Above_70Percent_Of_Average_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "73", AverageWait = "4", SafeLow = "61", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(ethGasStationPrice.SafeLowGwei, actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.SafeLowWait.AsInt(), 0), actualResult.WaitTime);
        }

        [TestMethod]
        public void Calling_GetFreePlanGwei_Eth_Client_Returns_Null_Should_Return_Null()
        {
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(null)));

            var actualResult = etheHelper.GetFreePlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(null, actualResult);
            loggerMock.Verify(s => s.Information(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_GetPaidPlanGwei_With_Null_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetPaidPlanGwei(null, default).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetPaidPlanGwei_With_Empty_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetPaidPlanGwei(string.Empty, default).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_GetPaidPlanGwei_With_WhiteSpace_ApiEndPoint_Should_Throw_An_Exception()
        {
            etheHelper.GetPaidPlanGwei(" ", default).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Calling_GetPaidPlanGwei_Eth_Client_Returns_Null_Should_Return_Null()
        {
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(null)));

            var actualResult = etheHelper.GetPaidPlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(null, actualResult);
            loggerMock.Verify(s => s.Information(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Calling_GetPaidPlanGwei_HappyPath()
        {
            var ethGasStationPrice = new EthGasStationPrice { Fast = "36", FastWait = "1", Average = "36", AverageWait = "4", SafeLow = "36", SafeLowWait = "25" };
            ethClientMock.Setup(s => s.GetJsonResponseContent(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(JsonConvert.SerializeObject(ethGasStationPrice)));

            var actualResult = etheHelper.GetPaidPlanGwei("dummyEndPoint", default).GetAwaiter().GetResult();

            Assert.AreEqual(ethGasStationPrice.FastGwei, actualResult.Gwei);
            Assert.AreEqual(new TimeSpan(0, ethGasStationPrice.FastWait.AsInt(), 0), actualResult.WaitTime);
        }
    }
}