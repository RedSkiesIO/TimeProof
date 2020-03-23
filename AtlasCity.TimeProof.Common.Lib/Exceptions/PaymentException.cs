using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class PaymentException : Exception
    {
        public PaymentException(string message) : base(message)
        {
        }

        public PaymentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PaymentException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}