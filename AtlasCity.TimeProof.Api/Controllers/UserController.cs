using System.Threading;
using AtlasCity.TimeProof.Abstractions.Requests;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using Dawn;
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
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userService, nameof(userService)).NotNull();
            Guard.Argument(userSubscriptionService, nameof(userSubscriptionService)).NotNull();

            _logger = logger;
            _userService = userService;
            _userSubscriptionService = userSubscriptionService;
        }

        [Route("user")]
        [HttpGet]
        public IActionResult Get([FromQuery] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest();

            var user = _userService.GetUserByEmail(email, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(user.ToResponse());
        }

        [Route("user")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest user, CancellationToken cancellationToken)
        {
            if (user == null)
                return BadRequest();

            var newUser = _userService.CreateUser(user.ToDao(), cancellationToken).GetAwaiter().GetResult();
            return new CreatedActionResult(newUser.ToResponse());
        }

        [Route("user/paymentintent/{planId}/{id}")]
        [HttpGet]
        public IActionResult GetPaymentIntent([FromRoute] string planId, string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(planId) || string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var response = _userSubscriptionService.GetPaymentIntent(id, planId, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
        }

        /// <summary>
        /// Once payment is successful on client side. Make record on server
        /// </summary>
        /// <param name="piid">Payment Intent Id</param>
        /// <param name="priceplanid">Price Plan Id</param>
        /// <param name="id">User Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("user/payment/{piid}/{ppid}/{id}")]
        [HttpPut]
        public IActionResult PaymentSuccess([FromRoute] string piid, string ppid, string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(piid) || string.IsNullOrWhiteSpace(ppid) || string.IsNullOrWhiteSpace(id))
                return BadRequest();

            _userSubscriptionService.ProcessPayment(id, piid, ppid, cancellationToken).GetAwaiter().GetResult();
            return new OkResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">UserId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("user/paymentmethod/{id}")]
        [HttpGet]
        public IActionResult GetCustomerPaymentMethod([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var response = _userSubscriptionService.GetCustomerPaymentMethod(id, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ppid">New Price Plan Id</param>
        /// <param name="id">UserId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("user/upgrade/{ppid}/{id}")]
        [HttpPut]
        public IActionResult UpgradePricePlan([FromRoute] string ppid, [FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            _userSubscriptionService.UpgradePricePlan(id, ppid, cancellationToken).GetAwaiter().GetResult();
            return new OkResult();
        }

        [Route("user/setupintent/{id}")]
        [HttpGet]
        public IActionResult GetSetupIntent([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var response = _userSubscriptionService.CreateSetupIntent(id, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
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
    }
}