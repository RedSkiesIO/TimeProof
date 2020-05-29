using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class RpcClientUnderpricedException : Exception
    {
        public RpcClientUnderpricedException(string message) : base(message)
        {
        }

        public RpcClientUnderpricedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RpcClientUnderpricedException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}