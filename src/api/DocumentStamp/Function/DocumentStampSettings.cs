using System;
using System.IO;
using Catalyst.Abstractions.Cryptography;
using DocumentStamp.Http.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp.Function
{
    public class DocumentStampSettings
    {
        private readonly IPrivateKey _privateKey;

        public DocumentStampSettings(IPrivateKey privateKey)
        {
            _privateKey = privateKey;
        }

        [FunctionName("DocumentStampSettings")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DocumentStampSettings processing a request");

            try
            {
                var publicKey = _privateKey.GetPublicKey();

                return new OkObjectResult(new Result<object>(true,
                    new {PublicKey = publicKey.Bytes.ToBase32().ToUpper()}));
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