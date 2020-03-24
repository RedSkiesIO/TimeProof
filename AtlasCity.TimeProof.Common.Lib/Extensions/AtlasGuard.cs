using Dawn;

namespace AtlasCity.TimeProof.Common.Lib.Extensions
{
    public static class AtlasGuard
    {
        public static void IsNullOrWhiteSpace(this string stringValue)
        {
            Guard.Argument(stringValue, nameof(stringValue)).NotNull("value is null");
            Guard.Argument(stringValue, nameof(stringValue)).NotEmpty("value is empty");
            Guard.Argument(stringValue, nameof(stringValue)).NotWhiteSpace("value has only white spaces");
        }
    }
}
