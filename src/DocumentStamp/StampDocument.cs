using System.IO;
using System.Threading.Tasks;
using Autofac;
using Catalyst.Core.Lib;
using Catalyst.Core.Modules.Rpc.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DocumentStamp
{
    public static class StampDocument
    {
        [FunctionName("StampDocumentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            //var containerBuilder = new ContainerBuilder();
            //containerBuilder.RegisterModule<CoreLibProvider>();
            //containerBuilder.RegisterModule<RpcClientModule>();
            //var container = containerBuilder.Build();
            //var rpcClient = container.Resolve<RpcClient>();

            log.LogInformation("StampDocument processed a request");

            string name = req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult) new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}