using System;
using System.IO;
using System.Threading.Tasks;
using DocumentStamp.Http.Request;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using DocumentStamp.Validator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp
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

            try
            {
                var stampDocumentRequest =
                    ModelValidator.ValidateAndConvert<StampDocumentRequest>(await req.ReadAsStringAsync());

                //stampDocumentRequest.PublicKey = "EQWWLY36L3V6OG4UQQKHVIY3ORLFNWZLBNOZBCZWOJX2AT7F3URA".ToLower();
                //stampDocumentRequest.Hash =
                //    "4WT6N3R3CQL2QR3X2QA7SN374VN5FQEHYKUXTEXTDA4JWHBZHULSUOGVST6HCAMKELO467I2P24H4MJ3IUTGGGR4YUPUHANTDWMYQSA"
                //        .ToLower();
                //stampDocumentRequest.Signature =
                //    "BGEKWDCR2SLIXKZZGSPRN3ZKKWYGNTGRU2PKO4BLXQJVJVL6NJXFJXLPVVGKBER6FIJTWMKCJ4OXKBLJGEHVT55ODK2OEICR6QF5SDY"
                //        .ToLower();


                var signature = stampDocumentRequest.Signature.FromBase32();
                var message = stampDocumentRequest.Hash.FromBase32();
                var publicKey = new Ed25519PublicKeyParameters(stampDocumentRequest.PublicKey.FromBase32(), 0);

                var verifier = new Ed25519Signer();
                verifier.Init(false, publicKey);
                verifier.BlockUpdate(message, 0, message.Length);
                var shouldVerify = verifier.VerifySignature(signature);
                if (!shouldVerify)
                {
                    throw new InvalidDataException("Could not verify signature");
                }

                var stampDocumentResponse = new StampDocumentResponse
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TimeStamp = DateTime.UtcNow,
                    UserProof = stampDocumentRequest,
                    NodeProof = new NodeProof {PublicKey = "test pub key", Signature = "test sig"}
                };

                return new OkObjectResult(new Result<StampDocumentResponse>(true, stampDocumentResponse));
            }
            catch (InvalidDataException ide)
            {
                return new BadRequestObjectResult(new Result<string>(false, ide.Message));
            }
        }
    }
}