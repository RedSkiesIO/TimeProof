using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class VerifyController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITimestampService _timestampService;

        public VerifyController(ILogger logger, ITimestampService timestampService)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(timestampService, nameof(timestampService)).NotNull();

            _logger = logger;
            _timestampService = timestampService;
        }

        [Route("verify/{pubKey}/{fileHash}")]
        [HttpGet]
        public IActionResult GetTimeStamp([FromRoute] string pubKey, [FromRoute] string fileHash, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(pubKey))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(fileHash))
                return BadRequest();

            try
            {
                var timestamp = _timestampService.VerifyTimestamp(pubKey, fileHash, cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch (TimestampException)
            {
                return new ConflictResult();
            }
        }
    }
}