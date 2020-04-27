using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Dawn;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class EthHelper : IEthHelper
    {
        private readonly EthSettings _ethSettings;

        public EthHelper(EthSettings ethSettings)
        {
            Guard.Argument(ethSettings, nameof(ethSettings)).NotNull();

            _ethSettings = ethSettings;
        }

        public bool VerifyStamp(TimestampDao timestamp)
        {
            var fileHash = timestamp.FileHash.ToUpper().FromBase32();
            var signature = timestamp.Signature.ToUpper().FromBase32();
            var publicKey = new Ed25519PublicKeyParameters(timestamp.PublicKey.ToUpper().FromBase32(), 0);

            var verifier = new Ed25519Signer();
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(fileHash, 0, fileHash.Length);
            return verifier.VerifySignature(signature);
        }

        public EthSettings GetEthSettings()
        {
            return _ethSettings;
        }
    }
}