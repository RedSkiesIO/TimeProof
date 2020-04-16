using System;
using AtlasCity.TimeProof.Abstractions.Helpers;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class SystemDateTime : ISystemDateTime
    {
        public DateTime GetUtcDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
