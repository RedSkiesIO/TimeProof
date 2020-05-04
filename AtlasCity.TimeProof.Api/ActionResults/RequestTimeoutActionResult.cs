using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasCity.TimeProof.Api.ActionResults
{
    public class RequestTimeoutActionResult : ObjectResult
    {
        public RequestTimeoutActionResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.RequestTimeout;
        }
    }
}
