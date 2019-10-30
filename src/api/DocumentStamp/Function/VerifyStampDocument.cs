using System;
using System.IO;
using System.Threading.Tasks;
using DocumentStamp.Helper;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace DocumentStamp.Function
{
    public class VerifyStampDocument
    {
        [FunctionName("VerifyStampDocumentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "VerifyStampDocument/{txId}")]
            HttpRequest req,
            string txId,
            ILogger log)
        {
            log.LogInformation("VerifyStampDocument processing a request");

            try
            {
                var configRoot = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json")).Build();

                var config = new Config(configRoot);
                var stampDocumentResponse =
                    HttpHelper.GetStampDocument($"http://{config.NodeConfig.IpAddress}:{5005}", txId);
                return new OkObjectResult(new Result<StampDocumentResponse>(true, stampDocumentResponse));
            }
            catch (InvalidDataException ide)
            {
                return new BadRequestObjectResult(new Result<string>(false, ide.Message));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new Result<string>(false, exc.Message));
            }
        }
    }
}