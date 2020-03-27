using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasCity.TimeProof.Api.ActionResults
{
    public class NoContentActionResult : ObjectResult
    {
        public NoContentActionResult() : base(null)
        {
            StatusCode = (int)HttpStatusCode.NoContent;
        }
    }
}