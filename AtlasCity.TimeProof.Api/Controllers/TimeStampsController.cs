using System.Threading;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TimeStampsController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITimestampService _timestampService;

        public TimeStampsController(ILogger logger, ITimestampService timestampService)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(timestampService);

            _logger = logger;
            _timestampService = timestampService;
        }


        //TODO: Sudhir Paging
        [Route("gettimestamps/{id}")]
        [HttpGet]
        //[Authorize]
        public IActionResult Get([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            return new SuccessActionResult(_timestampService.GetUesrTimestamps(id, cancellationToken).GetAwaiter().GetResult());
        }

        [Route("timestamp")]
        [HttpPost]
        public IActionResult AddTimeStamp([FromBody] TimestampDao newTimestamp, CancellationToken cancellationToken)
        {
            if (newTimestamp == null)
                return BadRequest();

            var newTimeStamp = _timestampService.GenerateTimestamp(newTimestamp.UserId, newTimestamp, cancellationToken).GetAwaiter().GetResult();

            return new CreatedActionResult(newTimeStamp);
        }
    }
}