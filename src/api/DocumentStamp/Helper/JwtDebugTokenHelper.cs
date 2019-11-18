using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DocumentStamp.Helper
{
    public static class JwtDebugTokenHelper
    {
        public static ClaimsPrincipal GenerateClaimsPrincipal()
        {
            var jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL3RpbWVzdGFtcGVyLmIyY2xvZ2luLmNvbS9mYTE2MmRkZC00NGM1LTQ3Y2EtOTA5Mi1kZjAzMGYxZDg4ZmYvdjIuMC8iLCJleHAiOjE1NzM2NTYwNTIsIm5iZiI6MTU3MzY1MjQ1MiwiYXVkIjoiZmM1MzMxN2YtZTczMi00MTYwLWI3ODItYjAyNzRkODcwMjg3Iiwic3ViIjoiN2NjMThkYzgtMmI5NS00OWRkLWI5ODQtYTY3ODc3MTFkMmZlIiwiZ2l2ZW5fbmFtZSI6IlNhdG9zaGkiLCJmYW1pbHlfbmFtZSI6Ik5ha2Ftb3RvIiwiZW1haWxzIjpbInN0ZXBoZW4uaG9yc2ZhbGxAYXRsYXNjaXR5LmlvIl0sInRmcCI6IkIyQ18xX1RpbWVzdGFtcFNpZ25VcFNpZ25JbiIsIm5vbmNlIjoiNzgwZmUzYzAtMWYxZC00MTVhLWI1MDAtOGFhZjlhMTAzMzliIiwic2NwIjoiZGVtby5yZWFkIiwiYXpwIjoiZTRhYjVjMTMtNmE3Ni00Nzg0LWEwZjMtZGE3ZDUwMTM1ZmEzIiwidmVyIjoiMS4wIiwiaWF0IjoxNTczNjUyNDUyfQ.jOdpq2z1gGcyKjeqrGYDOFT7VmRf7GdDUOkjPWSVLeSFYV2FJiKOqfL0HqcRFhKJNMFJNnecHzzPMwJHjwVWZ7CeMf3WTChpSedPHMm5sCLKFtZWbLS1kqlNUSEAVqRX04x-jyXgo81hVmnS8KCCjIJ6R2V940JPVSy9Bzafkro-Azrj3npQC3OXO7AKGuh3fDUCZAOfRw2lqWD9RCzG5PLMFlxR2-geVx1c_tkF0UzJfxQ5Qf2QTjFYlJboczE0DmvdpMvHct21M6vck6yngQGNb9AWXpkhIS5LgpRBJ1NxLdj60C75JR3Y_IOyDXLCrkdsCLVk__sTIiBq5jFV8Q";
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return new ClaimsPrincipal(new ClaimsIdentity(token.Claims, "aad"));
        }
    }
}
