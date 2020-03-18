using System.Threading;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TimeStampsController : Controller
    {
        private readonly ILogger<TimeStampsController> _logger;
        private readonly ITimestampRepository _timestampRepository;

        public TimeStampsController(ILogger<TimeStampsController> logger, ITimestampRepository timestampRepository)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampRepository, nameof(timestampRepository)).NotNull();

            _logger = logger;
            _timestampRepository = timestampRepository;
        }

        [Route("gettimestamps/{id}")]
        [HttpGet]
        // [Authorize]
        public JsonResult Get([FromRoute] string id, CancellationToken cancellationToken)
        {
            return Json(_timestampRepository.GetTimestampByUser(id, cancellationToken).GetAwaiter().GetResult());
        }

        [Route("timestamp")]
        [HttpPost]
        public JsonResult AddTimeStamp([FromBody] TimestampDao newTimestamp, CancellationToken cancellationToken)
        {
            newTimestamp.UserId = "182225ec-ab88-4821-a2b7-31061de06090";
            var newTimeStamp = _timestampRepository.CreateTimestamp(newTimestamp, cancellationToken).GetAwaiter().GetResult();

            return Json(newTimeStamp);
        }
    }
}