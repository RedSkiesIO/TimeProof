using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AtlasCity.TimeProof.Api.ActionResults;

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
        //[Authorize]
        public IActionResult Get([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            return new SuccessActionResult(_timestampRepository.GetTimestampByUser(id, cancellationToken).GetAwaiter().GetResult());
        }

        [Route("timestamp")]
        [HttpPost]
        public IActionResult AddTimeStamp([FromBody] TimestampDao newTimestamp, CancellationToken cancellationToken)
        {
            if (newTimestamp == null)
                return BadRequest();

            newTimestamp.UserId = "3ad6293f-811b-4238-b7f6-278f842ade61";
            var newTimeStamp = _timestampRepository.CreateTimestamp(newTimestamp, cancellationToken).GetAwaiter().GetResult();

            return new CreatedActionResult(newTimeStamp);
        }
    }
}