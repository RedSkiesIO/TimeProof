using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasCity.TimeProof.Api.ActionResults
{
    public class SuccessActionResult : ObjectResult
    {
        public SuccessActionResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
