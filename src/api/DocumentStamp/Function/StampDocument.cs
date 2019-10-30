using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.P2P;
using Catalyst.Abstractions.Rpc;
using Catalyst.Abstractions.Types;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.IO.Messaging.Correlation;
using Catalyst.Core.Lib.IO.Messaging.Dto;
using Catalyst.Core.Modules.Cryptography.BulletProofs;
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
    public static class StampDocument
    {
        [FunctionName("StampDocumentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("StampDocument processing a request");

            var autoResetEvent = new AutoResetEvent(false);

            var container = AutoFacHelper.GenerateRpcClientContainerBuilder().Build();
            using (container.BeginLifetimeScope())
            {
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

                    var cryptoWrapper = new FfiWrapper();

                    //Resolve keys the azure function will use
                    var privateKey = container.Resolve<IKeyStore>().KeyStoreDecrypt(KeyRegistryTypes.DefaultKey);

                    var receiverPublicKey =
                        cryptoWrapper.GetPublicKeyFromBytes(stampDocumentRequest.PublicKey.FromBase32());

                    var config = container.Resolve<Config>();

                    //Resolve the rpc client
                    var rpcClient = container.Resolve<IRpcClient>();
                    var rpcClientSettings = container.Resolve<IRpcClientConfig>();
                    var recipientPeerid =
                        rpcClientSettings.PublicKey.BuildPeerIdFromBase32Key(rpcClientSettings.HostAddress,
                            rpcClientSettings.Port);

                    //Resolve the azure function peer settings
                    var peerSettings = container.Resolve<IPeerSettings>();

                    //Connect to the node
                    await rpcClient.StartAsync();

                    //Listen to BroadcastRawTransactionResponse responses from the node.
                    rpcClient.SubscribeToResponse<BroadcastRawTransactionResponse>(x =>
                    {
                        if (x.ResponseCode != ResponseCode.Successful)
                        {
                            throw new InvalidDataException(
                                $"Stamp document returned an invalid response code: {x.ResponseCode}");
                        }

                        autoResetEvent.Set();
                    });

                    //Construct DocumentStamp smart contract data
                    var userProofJson = JsonConvert.SerializeObject(stampDocumentRequest);
                    var transaction =
                        StampTransactionHelper.GenerateStampTransaction(privateKey, receiverPublicKey,
                            userProofJson.ToUtf8Bytes(), 1, 1);
                    var protocolMessage =
                        transaction.ToProtocolMessage(peerSettings.PeerId, CorrelationId.GenerateCorrelationId());

                    rpcClient.SendMessage(new MessageDto(protocolMessage, recipientPeerid));

                    //Wait for node response then generate azure function response
                    autoResetEvent.WaitOne();

                    var stampDocumentResponse =
                        HttpHelper.GetStampDocument($"http://{config.NodeConfig.IpAddress}:{5005}",
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
}