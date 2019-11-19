using System;
using System.Collections.Generic;
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
    public class GetStamps
    {
        private readonly CosmosDbRepository<DocumentStampMetaData> _documentStampMetaDataRepository;
        public GetStamps(CosmosDbRepository<DocumentStampMetaData> documentStampMetaDataRepository)
        {
            _documentStampMetaDataRepository = documentStampMetaDataRepository;
        }

        [FunctionName("GetStamps")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetStamps/{page}/{count}")]
            HttpRequest req,
            ClaimsPrincipal principal,
            int page,
            int count,
            ILogger log)
        {
            page--;
            string userId;
#if (DEBUG)
            principal = JwtDebugTokenHelper.GenerateClaimsPrincipal();
            userId = principal.Claims.First(x => x.Type == "sub").Value;
#else
            userId = principal.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
#endif

            log.LogInformation("GetStamps processing a request");

            try
            {
                var documentStamp = _documentStampMetaDataRepository.FindAll(x => x.User == userId).Skip(page * count).Take(count).OrderByDescending(x => x.StampDocumentProof.TimeStamp);
                var stampedDocumentList = documentStamp.Select(x => new StampDocumentResponse() { FileName = x.FileName, StampDocumentProof = x.StampDocumentProof }).ToList();

                return new OkObjectResult(new Result<IEnumerable<StampDocumentResponse>>(true, stampedDocumentList));
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