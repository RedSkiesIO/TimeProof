using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface IEthHelper
    {
        bool VerifyStamp(TimestampDao timestamp);

        EthSettings GetEthSettings();
    }
}
