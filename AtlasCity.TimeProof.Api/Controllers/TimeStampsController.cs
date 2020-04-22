using System.Threading;
using AtlasCity.TimeProof.Abstractions.Requests;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using Dawn;
using Microsoft.AspNetCore.Authorization;
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
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampService, nameof(timestampService)).NotNull();

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

            var timestamps = _timestampService.GetUesrTimestamps(id, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(timestamps.ToResponse());
        }

        [Route("timestamp")]
        [HttpPost]
        public IActionResult AddTimeStamp([FromBody] CreateTimestampRequest newTimestamp, CancellationToken cancellationToken)
        {
            if (newTimestamp == null)
                return BadRequest();

            var newTimeStamp = _timestampService.GenerateTimestamp(newTimestamp.ToDao(), cancellationToken).GetAwaiter().GetResult();

            return new CreatedActionResult(newTimeStamp.ToResponse());
        }

        [Route("timestamp/{tsId}")]
        [HttpGet]
        public IActionResult GetTimeStamp([FromRoute] string tsid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tsid))
                return BadRequest();

            var timestamp = _timestampService.GetTimestampDetails(tsid, cancellationToken).GetAwaiter().GetResult();

            return new SuccessActionResult(timestamp.ToResponse());
        }
    }
}