using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class TimestampException : Exception
    {
        public TimestampException(string message) : base(message)
        {
        }

        public TimestampException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TimestampException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}