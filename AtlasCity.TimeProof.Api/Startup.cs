using System;
using System.Net;
using System.Net.Mail;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.Extensions;
using AtlasCity.TimeProof.Common.Lib;
using AtlasCity.TimeProof.Common.Lib.Helpers;
using AtlasCity.TimeProof.Common.Lib.Services;
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
            var origins = new string[] { "http://localhost:6420", "http://86.23.42.81:6420", "http://192.168.0.25:6420", "https://timeproof.netlify.com" };
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

            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.WithOrigins("http://localhost:6240", "https://timeproof.netlify.com").AllowAnyHeader().AllowAnyMethod();
            //        });
            //});

            services.AddControllers();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            services.AddSingleton(Log.Logger);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISystemDateTime, SystemDateTime>();

            //services.AddAuthentication(sharedOptions =>
            //{
            //    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //.AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
            //.AddCookie();

            //services.AddAuthentication(sharedOptions =>
            //{
            //    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            //}).AddOpenIdConnect(options => {
            //    //options.Authority = "https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/b2c_1_signupsignin/oauth2/v2.0/authorize";
            //    options.Authority = "https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/b2c_1_signupsignin/v2.0/.well-known/openid-configuration";
            //    options.ClientId = "caead9d0-3263-42b9-b25e-2ca36d0ff535";
            //    options.ClientSecret = "KHTC4N=xG2NN59Ak:RzmzwSvs]EEi[EF";
            //    options.UseTokenLifetime = true;
            //});

            //services.AddAuthorization();

            //services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromHours(1);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});

            var endpointUrl = Configuration.GetSection("TransationCosmosDb:EndpointUrl").Value;
            var authorizationKey = Configuration.GetSection("TransationCosmosDb:AuthorizationKey").Value;
            services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IUserRepository>(new UserRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPricePlanRepository>(new PricePlanRepository(endpointUrl, authorizationKey));
            services.AddSingleton<IPaymentRepository>(new PaymentRepository(endpointUrl, authorizationKey));

            var client = new SmtpClient(Configuration.GetSection("SMTPEmail:HostName").Value, int.Parse(Configuration.GetSection("SMTPEmail:Port").Value));
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Configuration.GetSection("SMTPEmail:UserName").Value, Configuration.GetSection("SMTPEmail:Password").Value);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            services.AddSingleton<IEmailService>(new EmailService(client, Log.Logger));

            var timeProofLoginUri = Configuration.GetSection("TimeProofLoginUri").Value;
            services.AddSingleton<IEmailTemplateHelper>(new EmailTemplateHelper(timeProofLoginUri));


            var paymentApiKey = Configuration.GetSection("PaymentApiKey").Value;
            var stripeClient = new StripeClient(paymentApiKey);
            services.AddSingleton(new PaymentIntentService(stripeClient));
            services.AddSingleton(new CustomerService(stripeClient));
            services.AddSingleton(new PaymentMethodService(stripeClient));
            services.AddSingleton(new SetupIntentService(stripeClient));

            services.AddSingleton<IPaymentService, StripePaymentService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserSubscriptionService, UserSubscriptionService>();
            services.AddSingleton<ITimestampService, TimestampService>();           
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
            //app.UseSession();

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
