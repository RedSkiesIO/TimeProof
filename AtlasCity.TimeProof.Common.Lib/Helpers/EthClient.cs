using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EthClient : IEthClient
    {
        private readonly ILogger _logger;

        public EthClient(ILogger logger)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();

            _logger = logger;
        }

        public async Task<double> GetCryptoCurrencyValue(string crypoCurrencyCode, string currencyCode, CancellationToken cancellationToken)
        {
            var clientUri = $"https://min-api.cryptocompare.com/data/price?fsym={crypoCurrencyCode}&tsyms={currencyCode}";

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(clientUri, cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Warning($"Request to {client} resulted in status code {response.StatusCode}");
                    return 0;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information($"Response from {client} returned {responseContent}");

                var responseJson = JObject.Parse(responseContent);
                var currencyValue = double.Parse(responseJson.SelectToken(currencyCode).ToString());
                return currencyValue;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while getting response form {clientUri}. Error: {ex.Message}");
            }

            return 0;
        }

        public async Task<string> GetJsonResponseContent(string clientUri, CancellationToken cancellationToken)
        {
            Guard.Argument(clientUri, nameof(clientUri)).NotNull().NotEmpty().NotWhiteSpace();

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(clientUri, cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Warning($"Request to {client} resulted in status code {response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Verbose($"Response from {client} returned {responseContent}");

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while getting response form {clientUri}. Error: {ex.Message}");
            }

            return null;
        }
    }
}