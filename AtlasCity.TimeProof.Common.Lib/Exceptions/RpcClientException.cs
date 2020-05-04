using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class RpcClientException : Exception
    {
        public RpcClientException(string message) : base(message)
        {
        }

        public RpcClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RpcClientException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}