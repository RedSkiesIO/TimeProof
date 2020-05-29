using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class RpcClientNonceException : Exception
    {
        public RpcClientNonceException(string message) : base(message)
        {
        }

        public RpcClientNonceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RpcClientNonceException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}