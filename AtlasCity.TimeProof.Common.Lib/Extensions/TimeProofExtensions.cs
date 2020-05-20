using System;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class TimeProofExtensions
    {
        public static int AsInt(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return 0;

            if (int.TryParse(source, out int result))
                return result;

            return 0;
        }
       
        public static int ToEpoch(this DateTime date)
        {
            if (date == null) 
                    return int.MinValue;
            
            var  epoch = new DateTime(1970, 1, 1);
            var epochTimeSpan = date - epoch;
            return (int)epochTimeSpan.TotalSeconds;
        }

        public static string ToLongDateTimeString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm:ss.fff tt");
        }
    }
}
