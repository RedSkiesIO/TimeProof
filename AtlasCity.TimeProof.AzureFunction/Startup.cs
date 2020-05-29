using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Services;
using AtlasCity.TimeProof.Repository.CosmosDb;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Stripe;
using System;

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
                .WriteTo.File("function_log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, buffered: false)
                .CreateLogger();

            builder.Services.AddLogging(s => s.AddSerilog(Log.Logger));
            builder.Services.AddSingleton(Log.Logger);

            var veryfyTransactionSettings = new VeryfyTransactionSettings()
            {
                NetheriumPremiumAccountFromAddress = configuration.GetSection("NetheriumPremiumAccountFromAddress").Value,
                NetheriumBasicAccountFromAddress = configuration.GetSection("NetheriumBasicAccountFromAddress").Value,
                NetheriumAccountNodeEndpoint     = configuration.GetSection("NetheriumAccountNodeEndpoint").Value
            };

            builder.Services.AddSingleton(veryfyTransactionSettings);

            var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
            builder.Services.AddSingleton<ITimestampQueueService>(new TimestampQueueService(storageAccountConnectionString, Log.Logger));

            var endpointUrl = configuration.GetSection("TransationCosmosDbEndpointUrl").Value;
            var authorizationKey = configuration.GetSection("TransationCosmosDbAuthorizationKey").Value;

            builder.Services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));
            builder.Services.AddSingleton<IUserRepository>(new UserRepository(endpointUrl, authorizationKey));
            builder.Services.AddSingleton<IPricePlanRepository>(new PricePlanRepository(endpointUrl, authorizationKey));
            builder.Services.AddSingleton<IPaymentRepository>(new PaymentRepository(endpointUrl, authorizationKey));
            builder.Services.AddSingleton<IPendingMembershipChangeRepository>(new PendingMembershipChangeRepository(endpointUrl, authorizationKey));

            var paymentApiKey = configuration.GetSection("PaymentApiKey").Value;
            var stripeClient = new StripeClient(paymentApiKey);
            builder.Services.AddSingleton(new PaymentIntentService(stripeClient));
            builder.Services.AddSingleton(new CustomerService(stripeClient));
            builder.Services.AddSingleton(new PaymentMethodService(stripeClient));
            builder.Services.AddSingleton(new SetupIntentService(stripeClient));

            builder.Services.AddSingleton<IPaymentService, StripePaymentService>();
            builder.Services.AddSingleton<IUserSubscriptionService, UserSubscriptionService>();
        }
    }
}