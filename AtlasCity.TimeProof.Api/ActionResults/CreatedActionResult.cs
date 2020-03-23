using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasCity.TimeProof.Api.ActionResults
{
    public class CreatedActionResult : ObjectResult
    {
        public CreatedActionResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.Created;
        }
    }
}
