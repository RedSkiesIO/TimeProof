using System;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
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

        // This method gets called by the runtime. Use this method to add services to the container.
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

            //// Adds a default in-memory implementation of IDistributedCache.
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

            var paymentApiKey = Configuration.GetSection("PaymentApiKey").Value;
            var stripeClient = new StripeClient(paymentApiKey);
            services.AddSingleton(new PaymentIntentService(stripeClient));
            services.AddSingleton(new CustomerService(stripeClient));

            services.AddSingleton<IPaymentService, StripePaymentService>();
            services.AddSingleton<IUserService, UserService>();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();
            ////app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    RequireHeaderSymmetry = false,
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
