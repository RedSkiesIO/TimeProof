using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }

        public UserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}