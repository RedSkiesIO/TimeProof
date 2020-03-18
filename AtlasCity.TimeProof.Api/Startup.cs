using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Repository.CosmosDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            //.AddCookie()
            //.AddOpenIdConnect(options =>
            //{
            //    options.ClientId = "ef2503cc-df85-41b5-b7c2-0b965e12ec1d";
            //    options.ClientSecret = "8.Q9tzIl7NGu=nt8rwwyymJgT=ichwM=";
            //    options.Authority = "https://login.microsoftonline.com/timestamper.onmicrosoft.com";

            //    options.ResponseType = "code";
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //});

        //    services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
        //.AddAzureADBearer(options => configuration.Bind("AzureAd", options));

            var endpointUrl = Configuration.GetSection("TransationCosmosDb:EndpointUrl").Value;
            var authorizationKey = Configuration.GetSection("TransationCosmosDb:AuthorizationKey").Value;
            services.AddSingleton<ITimestampRepository>(new TimestampRepository(endpointUrl, authorizationKey));

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

            app.UseAuthorization();

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
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
