using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;

namespace DocumentStamp.Helper
{
    public static class SignatureHelper
    {
        public static bool Verify(byte[] message, byte[] signature, ICipherParameters publicKey)
        {
            var verifier = new Ed25519Signer();
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
    }
}