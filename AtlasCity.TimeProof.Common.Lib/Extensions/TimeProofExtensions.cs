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
    }
}
