using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class PaymentServiceException : Exception
    {
        public PaymentServiceException(string message) : base(message)
        {
        }

        public PaymentServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PaymentServiceException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}