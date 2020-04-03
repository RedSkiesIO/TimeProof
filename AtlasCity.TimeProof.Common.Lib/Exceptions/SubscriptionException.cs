using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class SubscriptionException : Exception
    {
        public SubscriptionException(string message) : base(message)
        {
        }

        public SubscriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SubscriptionException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}