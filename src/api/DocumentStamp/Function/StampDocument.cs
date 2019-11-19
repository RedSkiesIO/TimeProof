using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Core.Lib.DAO;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Modules.Cryptography.BulletProofs;
using Catalyst.Modules.Repository.CosmosDb;
using Catalyst.Protocol.Peer;
using DocumentStamp.Helper;
using DocumentStamp.Http.Request;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using DocumentStamp.Validator;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp.Function
{
    //https://github.com/Azure/DotNetty/issues/246 memoryleaks in dotnetty, using web3 instead until fixed
    public class StampDocument
    {
        private readonly RestClient _restClient;
        private readonly PeerId _peerId;
        private readonly FfiWrapper _cryptoContext;
        private readonly IPrivateKey _privateKey;
        private readonly CosmosDbRepository<DocumentStampMetaData> _documentStampMetaDataRepository;

        public StampDocument(RestClient restClient, PeerId peerId, FfiWrapper cryptoContext, IPrivateKey privateKey, CosmosDbRepository<DocumentStampMetaData> documentStampMetaDataRepository)
        {
            _restClient = restClient;
            _peerId = peerId;
            _cryptoContext = cryptoContext;
            _privateKey = privateKey;
            _documentStampMetaDataRepository = documentStampMetaDataRepository;
        }

        [FunctionName("StampDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ClaimsPrincipal principal,
            ILogger log)
        {
            string userId;
#if (DEBUG)
            //Test JWT token
            principal = JwtDebugTokenHelper.GenerateClaimsPrincipal();
            userId = principal.Claims.First(x => x.Type == "sub").Value;
#else
            //Actual JWT token
            userId = principal.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
#endif

            log.LogInformation("StampDocument processing a request");

            try
            {
                //Validate the request model sent by the user or client
                var stampDocumentRequest =
                    ModelValidator.ValidateAndConvert<StampDocumentRequest>(await req.ReadAsStringAsync());

                stampDocumentRequest.PublicKey = stampDocumentRequest.PublicKey.ToLowerInvariant();
                stampDocumentRequest.Signature = stampDocumentRequest.Signature.ToLowerInvariant();
                stampDocumentRequest.Hash = stampDocumentRequest.Hash.ToLowerInvariant();

                //Verify the signature of the stamp document request
                var verifyResult = SignatureHelper.VerifyStampDocumentRequest(stampDocumentRequest);
                if (!verifyResult)
                {
                    throw new InvalidDataException("Could not verify signature of document stamp request");
                }

                var receiverPublicKey =
                    _cryptoContext.GetPublicKeyFromBytes(stampDocumentRequest.PublicKey.FromBase32());

                //Construct DocumentStamp smart contract data
                var userProofJson = JsonConvert.SerializeObject(stampDocumentRequest);
                var transaction =
                    StampTransactionHelper.GenerateStampTransaction(_cryptoContext, _privateKey, receiverPublicKey,
                        userProofJson.ToUtf8Bytes(), 1, 1);
                var protocolMessage =
                    StampTransactionHelper.ConvertToProtocolMessage(transaction, _cryptoContext, _privateKey,
                        _peerId);

                var request = new RestRequest("/api/Mempool/AddTransaction", Method.POST);
                request.AddQueryParameter("transactionBroadcastProtocolBase64", Convert.ToBase64String(protocolMessage.ToByteArray()));

                var response = _restClient.Execute<TransactionBroadcastDao>(request);
                if (!response.IsSuccessful)
                {
                    throw new InvalidDataException("DocumentStamp failed to send");
                }

                var transactionId = transaction.Signature.RawBytes.ToByteArray().ToBase32().ToLowerInvariant();
                var stampDocumentResponse = new StampDocumentResponse
                {
                    StampDocumentProof = HttpHelper.GetStampDocument(_restClient, transactionId),
                    FileName = stampDocumentRequest.FileName
                };

                var documentStampMetaData = new DocumentStampMetaData
                {
                    Id = transactionId,
                    FileName = stampDocumentRequest.FileName,
                    PublicKey = stampDocumentRequest.PublicKey,
                    StampDocumentProof = stampDocumentResponse.StampDocumentProof,
                    User = userId
                };

                _documentStampMetaDataRepository.Add(documentStampMetaData);

                return new OkObjectResult(new Result<StampDocumentResponse>(true, stampDocumentResponse));
            }
            catch (InvalidDataException ide)
            {
                log.LogError(ide.ToString());
                return new BadRequestObjectResult(new Result<string>(false,
                    ide.Message));
            }
            catch (Exception exc)
            {
                log.LogError(exc.ToString());
                return new BadRequestObjectResult(new Result<string>(false,
                    exc.Message));
            }
        }
    }
}