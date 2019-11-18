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
using RestSharp;

namespace DocumentStamp.Function    
{
    public class VerifyStampDocument
    {
        private readonly RestClient _restClient;
        private readonly CosmosDbRepository<DocumentStampMetaData> _documentStampMetaDataRepository;
        public VerifyStampDocument(RestClient restClient, CosmosDbRepository<DocumentStampMetaData> documentStampMetaDataRepository)
        {
            _restClient = restClient;
            _documentStampMetaDataRepository = documentStampMetaDataRepository;
        }

        [FunctionName("VerifyStampDocument")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "VerifyStampDocument/{txId}")]
            HttpRequest req,
            ClaimsPrincipal principal,
            string txId,
            ILogger log)
        {
            string userId;
#if (DEBUG)
            principal = JwtDebugTokenHelper.GenerateClaimsPrincipal();
            userId = principal.Claims.First(x => x.Type == "sub").Value;
#else
            userId = principal.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
#endif

            log.LogInformation("VerifyStampDocument processing a request");

            try
            {
                var metaData = _documentStampMetaDataRepository.Find(x => x.Id == txId.ToLower() && x.User == userId);
                var stampDocumentResponse = new StampDocumentResponse
                {
                    StampDocumentProof = HttpHelper.GetStampDocument(_restClient, txId)
                };

                if (metaData != null)
                {
                    stampDocumentResponse.FileName = metaData.FileName;
                }

                return new OkObjectResult(new Result<StampDocumentResponse>(true, stampDocumentResponse));
            }
            catch (InvalidDataException ide)
            {
                log.LogError(ide.ToString());
                return new BadRequestObjectResult(new Result<string>(false, ide.Message));
            }
            catch (Exception exc)
            {
                log.LogError(exc.ToString());
                return new BadRequestObjectResult(new Result<string>(false, exc.Message));
            }
        }
    }
}