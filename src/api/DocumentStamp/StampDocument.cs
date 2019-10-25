using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp
{
    public static class StampDocument
    {
        private static bool VerifyStampDocumentRequest(UserProof userProof)
        {
            var hash = userProof.Hash.FromBase32();
            var signature = userProof.Signature.FromBase32();
            var publicKey = new Ed25519PublicKeyParameters(userProof.PublicKey.FromBase32(), 0);
            return SignatureHelper.Verify(hash, signature, publicKey);
        }

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

                var verifyResult = VerifyStampDocumentRequest(stampDocumentRequest);
                if (!verifyResult)
                {
                    throw new InvalidDataException("Could not verify signature of document stamp request");
                }

                var random = new SecureRandom();
                var keyPairGenerator = new Ed25519KeyPairGenerator();
                keyPairGenerator.Init(new Ed25519KeyGenerationParameters(random));
                var nodePublicKey = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPairGenerator.GenerateKeyPair().Public);

                var stampDocumentResponse = new StampDocumentResponse
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TimeStamp = DateTime.UtcNow,
                    UserProof = stampDocumentRequest,
                    NodeProof = new NodeProof {PublicKey = nodePublicKey.ToAsn1Object().GetDerEncoded().ToBase32().ToUpper(), Signature = Guid.NewGuid().ToByteArray().ToBase32().ToUpper()}
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