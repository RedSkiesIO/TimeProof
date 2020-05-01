using System.Threading;
using AtlasCity.TimeProof.Abstractions.Requests;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using AtlasCity.TimeProof.Api.Extensions;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
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
        [Route("gettimestamps")]
        [HttpGet]
        public IActionResult Get(CancellationToken cancellationToken)
        {
            var timestamps = _timestampService.GetUesrTimestamps(User.GetUserId(), cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(timestamps.ToResponse());
        }

        [Route("timestamp")]
        [HttpPost]
        public IActionResult AddTimeStamp([FromBody] CreateTimestampRequest newTimestamp, CancellationToken cancellationToken)
        {
            if (newTimestamp == null)
                return BadRequest();

            var timestampDao = newTimestamp.ToDao();
            timestampDao.UserId = User.GetUserId();

            try
            {
                var newTimeStamp = _timestampService.GenerateTimestamp(timestampDao, cancellationToken).GetAwaiter().GetResult();
                return new CreatedActionResult(newTimeStamp.ToResponse());
            }
            catch (TimestampException)
            {
                return new ConflictResult();
            }
        }

        [Route("timestamp/{tsId}")]
        [HttpGet]
        public IActionResult GetTimeStamp([FromRoute] string tsid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tsid))
                return BadRequest();

            try
            {
                var timestamp = _timestampService.GetTimestampDetails(tsid, User.GetUserId(), cancellationToken).GetAwaiter().GetResult();
                return new SuccessActionResult(timestamp.ToResponse());
            }
            catch (TimestampException)
            {
                return new ConflictResult();
            }
        }
    }
}