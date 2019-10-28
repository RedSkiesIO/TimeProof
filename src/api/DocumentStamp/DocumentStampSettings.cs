using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.Types;
using DocumentStamp.Helper;
using DocumentStamp.Http.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp
{
    public static class DocumentStampSettings
    {
        [FunctionName("DocumentStampSettingsFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DocumentStampSettings processing a request");

            var container = AutoFacHelper.GenerateRpcClientContainerBuilder().Build();
            using (container.BeginLifetimeScope())
            {
                try
                {
                    //Resolve keys the azure function will use
                    var privateKey = container.Resolve<IKeyStore>().KeyStoreDecrypt(KeyRegistryTypes.DefaultKey);
                    var publicKey = privateKey.GetPublicKey();

                    return new OkObjectResult(new Result<object>(true, new {PublicKey = publicKey.Bytes.ToBase32().ToUpper()}));
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
}