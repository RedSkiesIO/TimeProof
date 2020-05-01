using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class TimeScribeSecuityException : Exception
    {
        public TimeScribeSecuityException(string message) : base(message)
        {
        }

        public TimeScribeSecuityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TimeScribeSecuityException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}