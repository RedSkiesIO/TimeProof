using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Catalyst.Modules.Repository.CosmosDb;
using DocumentStamp.Helper;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DocumentStamp.Function
{
    public class GetTotalStamps
    {
        private readonly CosmosDbRepository<DocumentStampMetaData> _documentStampMetaDataRepository;
        public GetTotalStamps(CosmosDbRepository<DocumentStampMetaData> documentStampMetaDataRepository)
        {
            _documentStampMetaDataRepository = documentStampMetaDataRepository;
        }

        [FunctionName("GetTotalStamps")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ClaimsPrincipal principal,
            ILogger log)
        {
            string userId;
#if (DEBUG)
            principal = JwtDebugTokenHelper.GenerateClaimsPrincipal();
            userId = principal.Claims.First(x => x.Type == "sub").Value;
#else
            userId = principal.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
#endif

            log.LogInformation("GetTotalStamps processing a request");

            try
            {
                var documentStampCount = _documentStampMetaDataRepository.Count(x => x.User == userId);
                return new OkObjectResult(new Result<int>(true, documentStampCount));
            }
            catch (InvalidDataException ide)
            {
                log.LogError(ide.ToString());
                return new BadRequestObjectResult(new Result<string>(false, ide.ToString()));
            }
            catch (Exception exc)
            {
                log.LogError(exc.ToString());
                return new BadRequestObjectResult(new Result<string>(false, exc.ToString()));
            }
        }
    }
}