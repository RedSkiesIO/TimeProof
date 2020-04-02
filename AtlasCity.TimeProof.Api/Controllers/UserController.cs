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
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserController(ILogger logger, IUserService userService, IUserSubscriptionService userSubscriptionService)
        {
            AtlasGuard.IsNotNull(logger);
            AtlasGuard.IsNotNull(userService);
            AtlasGuard.IsNotNull(userSubscriptionService);

            _logger = logger;
            _userService = userService;
            _userSubscriptionService = userSubscriptionService;
        }

        [Route("user/{email}")]
        [HttpGet]
        public IActionResult Get([FromRoute] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest();

            var user = _userService.GetUserByEmail(email, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(user);
        }

        [Route("user")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDao user, CancellationToken cancellationToken)
        {
            if (user == null)
                return BadRequest();

            var newUser = _userService.CreateUser(user, cancellationToken).GetAwaiter().GetResult();
            return new CreatedActionResult(newUser);
        }

        [Route("user/{id}")]
        [HttpDelete]
        public IActionResult DeleteUser([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            _userService.DeleteUser(id, cancellationToken).GetAwaiter().GetResult();
            return new NoContentActionResult();
        }

        [Route("user/intent/{id}")]
        [HttpGet]
        public IActionResult GetSetupIntent([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var setupIntent = _userSubscriptionService.CreateSetupIntent(id, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(setupIntent);
        }

        [Route("user/payment")]
        [HttpPost]
        public IActionResult ProcessPayment([FromBody] PaymentDao payment, CancellationToken cancellationToken)
        {
            if (payment == null)
                return BadRequest();

            var paymentResponse = _userSubscriptionService.ProcessPayment(payment, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(paymentResponse);
        }
    }
}