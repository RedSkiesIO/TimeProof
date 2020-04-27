using System;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Services;
using AtlasCity.TimeProof.Repository.CosmosDb;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Serilog;

[assembly: FunctionsStartup(typeof(AtlasCity.TimeProof.AzureFunction.Startup))]
namespace AtlasCity.TimeProof.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Application", "TimeProof.Functions")
                .WriteTo.Console()
                .WriteTo.File("function_log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            builder.Services.AddLogging(s => s.AddSerilog(Log.Logger));
            builder.Services.AddSingleton(Log.Logger);

            var accountAddress = configuration.GetSection("NetheriumAccountFromAddress").Value;
            var nodeEndpoint = configuration.GetSection("NetheriumAccountNodeEndpoint").Value;
            var account = new ManagedAccount(accountAddress, string.Empty);
            var web3 = new Web3(account, nodeEndpoint);
            builder.Services.AddSingleton<IWeb3>(web3);

            var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
            builder.Services.AddSingleton<ITimestampQueueService>(new TimestampQueueService(storageAccountConnectionString, Log.Logger));

            var endpointUrl = configuration.GetSection("TransationCosmosDbEndpointUrl").Value;
            var authorizationKey = configuration.GetSection("TransationCosmosDbAuthorizationKey").Value;
            builder.Services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));
        }
    }
}