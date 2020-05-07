using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasCity.TimeProof.Api.ActionResults
{
    public class ConflictActionResult : ObjectResult
    {
        public ConflictActionResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.Conflict;
        }
    }
}
