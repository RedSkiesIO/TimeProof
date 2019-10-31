using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Abstractions.P2P;
using Catalyst.Abstractions.Rpc;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.IO.Messaging.Correlation;
using Catalyst.Core.Lib.IO.Messaging.Dto;
using Catalyst.Protocol.Peer;
using Catalyst.Protocol.Rpc.Node;
using DocumentStamp.Helper;
using DocumentStamp.Http.Request;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using DocumentStamp.Validator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp.Function
{
    public class StampDocument
    {
        private readonly AutoResetEvent _autoResetEvent;
        private readonly Config _config;
        private readonly IPeerSettings _peerSettings;
        private readonly ICryptoContext _cryptoContext;
        private readonly IPrivateKey _privateKey;
        private readonly IRpcClient _rpcClient;
        private readonly PeerId _recipientPeer;

        public StampDocument(Config config, IPeerSettings peerSettings, ICryptoContext cryptoContext,
            IPrivateKey privateKey, IRpcClient rpcClient,
            PeerId recipientPeer)
        {
            _autoResetEvent = new AutoResetEvent(false);
            _config = config;
            _peerSettings = peerSettings;
            _cryptoContext = cryptoContext;
            _privateKey = privateKey;
            _rpcClient = rpcClient;
            _recipientPeer = recipientPeer;
        }

        [FunctionName("StampDocumentFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("StampDocument processing a request");

            try
            {
                //Validate the request model sent by the user or client
                var stampDocumentRequest =
                    ModelValidator.ValidateAndConvert<StampDocumentRequest>(await req.ReadAsStringAsync());

                //Verify the signature of the stamp document request
                var verifyResult = SignatureHelper.VerifyStampDocumentRequest(stampDocumentRequest);
                if (!verifyResult)
                {
                    throw new InvalidDataException("Could not verify signature of document stamp request");
                }

                var receiverPublicKey =
                    _cryptoContext.GetPublicKeyFromBytes(stampDocumentRequest.PublicKey.FromBase32());

                //Connect to the node
                await _rpcClient.StartAsync();

                //Listen to BroadcastRawTransactionResponse responses from the node.
                _rpcClient.SubscribeToResponse<BroadcastRawTransactionResponse>(x =>
                {
                    if (x.ResponseCode != ResponseCode.Successful)
                    {
                        throw new InvalidDataException(
                            $"Stamp document returned an invalid response code: {x.ResponseCode}");
                    }

                    _autoResetEvent.Set();
                });

                //Construct DocumentStamp smart contract data
                var userProofJson = JsonConvert.SerializeObject(stampDocumentRequest);
                var transaction =
                    StampTransactionHelper.GenerateStampTransaction(_privateKey, receiverPublicKey,
                        userProofJson.ToUtf8Bytes(), 1, 1);
                var protocolMessage =
                    transaction.ToProtocolMessage(_peerSettings.PeerId, CorrelationId.GenerateCorrelationId());

                _rpcClient.SendMessage(new MessageDto(protocolMessage, _recipientPeer));

                //Wait for node response then generate azure function response
                _autoResetEvent.WaitOne();

                var stampDocumentResponse =
                    HttpHelper.GetStampDocument(_config.NodeConfig.WebAddress,
                        transaction.Transaction.Signature.RawBytes.ToByteArray().ToBase32().ToUpperInvariant());
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