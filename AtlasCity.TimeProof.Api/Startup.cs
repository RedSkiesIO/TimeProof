using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.Extensions;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using AtlasCity.TimeProof.Common.Lib.Services;
using AtlasCity.TimeProof.Repository.CosmosDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.RPC.NonceServices;
using Nethereum.Web3;
using Serilog;
using Stripe;
using System.Net;
using System.Net.Mail;

namespace AtlasCity.TimeProof.Api
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "AllowAllHeaders";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var origins = Configuration.GetValueAsArray("AllowedOrigins", ",");
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddControllers();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            services.AddSingleton(Log.Logger);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISystemDateTime, SystemDateTime>();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = Configuration.GetValue("Authentication:Authority");
                jwtOptions.Events = new JwtBearerEvents { };
                jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = Configuration.GetValueAsList("Authentication:Audiences", ","),
                    ValidIssuers = Configuration.GetValueAsList("Authentication:Issuers", ","),
                };
            });

            var storageAccountConnectionString = Configuration.GetConnectionString("StorageAccount");
            services.AddSingleton<ITimestampQueueService>(new TimestampQueueService(storageAccountConnectionString, Log.Logger));

            var endpointUrl = Configuration.GetValue("TransationDbEndpointUrl");
            var authorizationKey = Configuration.GetValue("TransationDbAuthorizationKey");
            services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IUserRepository>(new UserRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPricePlanRepository>(new PricePlanRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IAddressNonceRepository>(new AddressNonceRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPaymentRepository>(new PaymentRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPendingMembershipChangeRepository>(new PendingMembershipChangeRepository(endpointUrl, authorizationKey));

            var client = new SmtpClient(Configuration.GetValue("SMTPEmail:HostName"), int.Parse(Configuration.GetValue("SMTPEmail:Port")))
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Configuration.GetValue("SMTPEmail:UserName"), Configuration.GetValue("SMTPEmailPassword")),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            services.AddSingleton<IEmailService>(new EmailService(client, Log.Logger));
            services.AddSingleton<IEthClient, EthClient>();

            var basicAccountSecretKey = Configuration.GetValue("NetheriumBasicAccountSecretKey");
            var premiumAccountSecretKey = Configuration.GetValue("NetheriumPremiumAccountSecretKey");
            var toAddress = Configuration.GetValue("NetheriumAccount:ToAddress");
            var networkName = Configuration.GetValue("NetheriumAccount:Network");
            var gasStationAPIEndpoint = Configuration.GetValue("NetheriumAccount:GasStationAPIEndpoint");

            var ethSetting = new EthSettings { ToAddress = toAddress, BasicAccountSecretKey = basicAccountSecretKey, PremiumAccountSecretKey = premiumAccountSecretKey,  Network = networkName, GasStationAPIEndpoint = gasStationAPIEndpoint };

            services.AddSingleton<IEthHelper>(provider => new EthHelper(ethSetting, provider.GetService<IEthClient>(), provider.GetService<ILogger>()));

            var timeProofLoginUri = Configuration.GetValue("TimeProofLoginUri");
            services.AddSingleton<IEmailTemplateHelper>(new EmailTemplateHelper(timeProofLoginUri));

            var paymentApiKey = Configuration.GetValue("StripePaymentApiKey");
            var stripeClient = new StripeClient(paymentApiKey);
            services.AddSingleton(new PaymentIntentService(stripeClient));
            services.AddSingleton(new CustomerService(stripeClient));
            services.AddSingleton(new PaymentMethodService(stripeClient));
            services.AddSingleton(new SetupIntentService(stripeClient));

            services.AddSingleton<IPaymentService, StripePaymentService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserSubscriptionService, UserSubscriptionService>();

            var nodeEndpoint = Configuration.GetValue("NetheriumAccount:NodeEndpoint");

            var basicAccountFromAddress = Configuration.GetValue("NetheriumAccount:BasicAccountFromAddress");
            var basicAccount = new Nethereum.Web3.Accounts.Account(basicAccountSecretKey);
            var basicAccountWeb3 = new Web3(basicAccount, nodeEndpoint);
            basicAccount.NonceService = new InMemoryNonceService(basicAccountFromAddress, basicAccountWeb3.Client);

            var premiumAccountFromAddress = Configuration.GetValue("NetheriumAccount:PremiumAccountFromAddress");
            var premiumAccount = new Nethereum.Web3.Accounts.Account(premiumAccountSecretKey);
            var premiumAccountWeb3 = new Web3(premiumAccount, nodeEndpoint);
            premiumAccount.NonceService = new InMemoryNonceService(premiumAccountFromAddress, premiumAccountWeb3.Client);

            services.AddSingleton<ITimestampService>(provider =>
            new TimestampService(provider.GetService<ILogger>(),
                provider.GetService<ITimestampRepository>(),
                provider.GetService<IUserRepository>(),
                provider.GetService<IPricePlanRepository>(),
                provider.GetService<IEthHelper>(),
                provider.GetService<ITimestampQueueService>(),
                basicAccountWeb3,
                premiumAccountWeb3));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseSerilogRequestLogging();
            app.ConfigureExceptionHandler(logger);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }     
    }
}
