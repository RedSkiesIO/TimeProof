using System;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class SignatureHelper : ISignatureHelper
    {
        public bool VerifyStamp(TimestampDao timestamp)
        {
            var hash = timestamp.Hash.ToUpper().FromBase32();
            var signature = timestamp.Signature.ToUpper().FromBase32();
            var publicKey = new Ed25519PublicKeyParameters(timestamp.PublicKey.ToUpper().FromBase32(), 0);
            return Verify(hash, signature, publicKey);
        }

        private bool Verify(byte[] message, byte[] signature, ICipherParameters publicKey)
        {
            var verifier = new Ed25519Signer();
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
    }
}
