using DocumentStamp.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp.Helper
{
    public static class SignatureHelper
    {
        public static bool VerifyStampDocumentRequest(UserProof userProof)
        {
            var hash = userProof.Hash.FromBase32();
            var signature = userProof.Signature.FromBase32();
            var publicKey = new Ed25519PublicKeyParameters(userProof.PublicKey.FromBase32(), 0);
            return Verify(hash, signature, publicKey);
        }

        public static bool Verify(byte[] message, byte[] signature, ICipherParameters publicKey)
        {
            var verifier = new Ed25519Signer();
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
    }
}