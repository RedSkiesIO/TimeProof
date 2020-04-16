using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Helpers
{
    public interface ISignatureHelper
    {
        bool VerifyStamp(TimestampDao timestamp);
    }
}
