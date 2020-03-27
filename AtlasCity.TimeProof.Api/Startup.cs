using System;
using System.IO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.Extensions;
using AtlasCity.TimeProof.Common.Lib;
using AtlasCity.TimeProof.Repository.CosmosDb;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Stripe;

namespace AtlasCity.TimeProof.Api
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddAuthentication(sharedOptions =>
            //{
            //    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //.AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
            //.AddCookie();

            //services.AddAuthorization();

            var endpointUrl = Configuration.GetSection("TransationCosmosDb:EndpointUrl").Value;
            var authorizationKey = Configuration.GetSection("TransationCosmosDb:AuthorizationKey").Value;
            services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IUserRepository>(new UserRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPricePlanRepository>(new PricePlanRepository(endpointUrl, authorizationKey));

            var paymentApiKey = Configuration.GetSection("PaymentApiKey").Value;
            var stripeClient = new StripeClient(paymentApiKey);
            services.AddSingleton(new PaymentIntentService(stripeClient));
            services.AddSingleton(new CustomerService(stripeClient));
            services.AddSingleton(new SetupIntentService(stripeClient));

            services.AddSingleton<IPaymentService, StripePaymentService>();
            services.AddSingleton<IUserService, UserService>();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            services.AddSingleton(Log.Logger);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();
            ////app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    RequireHeaderSymmetry = false,
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            app.ConfigureExceptionHandler(logger);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
