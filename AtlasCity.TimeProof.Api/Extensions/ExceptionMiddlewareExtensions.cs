using System.Net;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AtlasCity.TimeProof.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null && contextFeature.Error != null)
                    {
                        if (contextFeature.Error.GetType() == typeof(TimeScribeSecuityException))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }

                        logger.Error($"Something went wrong: {contextFeature.Error}");
                        await context.Response.WriteAsync($"{{\"statusCode\":\"{ context.Response.StatusCode }\",\"message\":\"{contextFeature.Error.Message}\"}}");
                    }
                });
            });
        }
    }
}