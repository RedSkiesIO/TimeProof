using System.Threading;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Requests;
using AtlasCity.TimeProof.Api.ActionResults;
using Dawn;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class PricePlanController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPricePlanRepository _pricePlanRepository;

        public PricePlanController(ILogger logger, IPricePlanRepository pricePlanRepository)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(pricePlanRepository, nameof(pricePlanRepository)).NotNull();

            _logger = logger;
            _pricePlanRepository = pricePlanRepository;
        }

        [Route("priceplans")]
        [HttpGet]
        public IActionResult Get(CancellationToken cancellationToken)
        {
            var pricePlans = _pricePlanRepository.GetPricePlans(cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(pricePlans.ToResponse());
        }
    }
}