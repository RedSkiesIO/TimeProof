using System;
using System.Linq;
using System.Security.Claims;
using AtlasCity.TimeProof.Common.Lib.Exceptions;

namespace AtlasCity.TimeProof.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        private const string NameIdentifier = "nameidentifier";

        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                throw new TimeScribeSecuityException($"Unknown user.'");

            if (!claimsPrincipal.Claims.Any())
                throw new TimeScribeSecuityException($"Missing claims for the user '{claimsPrincipal.Identity}'.");

            var subjectClaim = claimsPrincipal.Claims.FirstOrDefault(s => s.Type.Contains(NameIdentifier, StringComparison.InvariantCultureIgnoreCase));
            if (subjectClaim == null)
                throw new TimeScribeSecuityException($"Missing claim type which contains '{NameIdentifier}' for the user '{claimsPrincipal.Identity}'.");

            return subjectClaim.Value;
        }
    }
}   