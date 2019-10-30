using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalyst.Core.Lib.DAO;
using DocumentStamp.Helper;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

                var client = new RestClient($"http://{config.NodeConfig.IpAddress}:{5005}");
                var request = new RestRequest("/api/Mempool/Get/{id}", Method.GET);
                request.AddUrlSegment("id", txId);

                var response = client.Execute<TransactionBroadcastDao>(request);
                var transactionBroadcastDao = response.Data;
                if (response.Data == null)
                {
                    throw new InvalidDataException("DocumentStamp does not exist under txId");
                }

                var smartContract = transactionBroadcastDao.ContractEntries.First();
                var smartContractData = Encoding.UTF8.GetString(Convert.FromBase64String(smartContract.Data));
                var stampedUserProof = JsonConvert.DeserializeObject<StampedUserProof>(smartContractData);

                //Verify the signature of the stamp document request
                var verifyResult = SignatureHelper.VerifyStampDocumentRequest(stampedUserProof.UserProof);
                if (!verifyResult)
                {
                    throw new InvalidDataException("Could not verify signature of document stamp request");
                }

                var stampDocumentResponse = new StampDocumentResponse
                {
                    TransactionId = transactionBroadcastDao.Id.ToUpper(),
                    TimeStamp = stampedUserProof.TimeStamp,
                    UserProof = stampedUserProof.UserProof,
                    NodeProof = new NodeProof
                    {
                        PublicKey = smartContract.Base.SenderPublicKey.ToUpper(),
                        Signature = transactionBroadcastDao.Signature.RawBytes.ToUpper()
                    }
                };
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