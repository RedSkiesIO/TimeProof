using System.Threading;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Api.ActionResults;
using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class PricePlanController : Controller
    {
        private readonly ILogger<PricePlanController> _logger;
        private readonly IPricePlanRepository _pricePlanRepository;

        public PricePlanController(ILogger<PricePlanController> logger, IPricePlanRepository pricePlanRepository)
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
            return new SuccessActionResult(_pricePlanRepository.GetPricePlans(cancellationToken).GetAwaiter().GetResult());
        }
    }
}