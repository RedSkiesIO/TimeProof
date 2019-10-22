using System;
using System.IO;
using System.Threading.Tasks;
using DocumentStamp.Model;
using DocumentStamp.Request;
using DocumentStamp.Response;
using DocumentStamp.Validator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

//using Autofac;
//using Catalyst.Core.Lib;
//using Catalyst.Core.Modules.Rpc.Client;

//var containerBuilder = new ContainerBuilder();
//containerBuilder.RegisterModule<CoreLibProvider>();
//containerBuilder.RegisterModule<RpcClientModule>();
//var container = containerBuilder.Build();
//var rpcClient = container.Resolve<RpcClient>();

namespace DocumentStamp
{
    public static class StampDocument
    {
        [FunctionName("StampDocumentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("StampDocument processing a request");

            try
            {
                var stampDocumentRequest =
                    ModelValidator.ValidateAndConvert<StampDocumentRequest>(await req.ReadAsStringAsync());

                var stampDocumentResponse = new StampDocumentResponse
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TimeStamp = DateTime.UtcNow,
                    UserProof = stampDocumentRequest,
                    NodeProof = new NodeProof {PublicKey = "test pub key", Signature = "test sig"}
                };

                return new OkObjectResult(new Result<StampDocumentResponse>(true, stampDocumentResponse));
            }
            catch (InvalidDataException ide)
            {
                return new BadRequestObjectResult(new Result<string>(false, ide.Message));
            }
        }
    }
}
