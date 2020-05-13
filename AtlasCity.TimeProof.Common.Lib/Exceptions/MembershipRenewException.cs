using System;

namespace AtlasCity.TimeProof.Common.Lib.Exceptions
{
    public class MembershipRenewException : Exception
    {
        public MembershipRenewException(string message) : base(message)
        {
        }

        public MembershipRenewException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MembershipRenewException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}