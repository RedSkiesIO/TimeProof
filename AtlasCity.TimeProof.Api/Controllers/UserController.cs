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
        public IActionResult Get(CancellationToken cancellationToken)
        {
            var user = _userService.GetUserById(User.GetUserId(), cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(user.ToResponse());
        }

        [Route("user")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest user, CancellationToken cancellationToken)
        {
            if (user == null)
                return BadRequest();

            var userDao = user.ToDao();
            userDao.Id = User.GetUserId();
            var newUser = _userService.CreateUser(userDao, cancellationToken).GetAwaiter().GetResult();
            return new CreatedActionResult(newUser.ToResponse());
        }

        [Route("user/paymentintent/{planId}")]
        [HttpGet]
        public IActionResult GetPaymentIntent([FromRoute] string planId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest();

            var response = _userSubscriptionService.GetPaymentIntent(User.GetUserId(), planId, cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
        }

        /// <summary>
        /// Once payment is successful on client side. Make record on server
        /// </summary>
        /// <param name="piid">Payment Intent Id</param>
        /// <param name="priceplanid">Price Plan Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("user/payment/{piid}/{ppid}")]
        [HttpPut]
        public IActionResult PaymentSuccess([FromRoute] string piid, string ppid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(piid) || string.IsNullOrWhiteSpace(ppid))
                return BadRequest();

            _userSubscriptionService.ProcessPayment(User.GetUserId(), piid, ppid, cancellationToken).GetAwaiter().GetResult();
            return new OkResult();
        }

        [Route("user/paymentmethod")]
        [HttpGet]
        public IActionResult GetCustomerPaymentMethod(CancellationToken cancellationToken)
        {
            var response = _userSubscriptionService.GetCustomerPaymentMethod(User.GetUserId(), cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
        }

        [Route("user/upgrade/{ppid}")]
        [HttpPut]
        public IActionResult ChanePricePlan([FromRoute] string ppid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ppid))
                return BadRequest();

            try
            {
                _userSubscriptionService.ChangePricePlan(User.GetUserId(), ppid, cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch(SubscriptionException ex)
            {
                return new ConflictActionResult(ex.Message);
            }
        }

        [Route("user/setupintent")]
        [HttpGet]
        public IActionResult GetSetupIntent(CancellationToken cancellationToken)
        {
            var response = _userSubscriptionService.CreateSetupIntent(User.GetUserId(), cancellationToken).GetAwaiter().GetResult();
            return new SuccessActionResult(response);
        }

        [Route("user/sendkey")]
        [HttpPost]
        public IActionResult SendKey([FromBody] object keyStore, CancellationToken cancellationToken)
        {
            if (keyStore == null)
                return BadRequest();

            try { 
            _userService.SendKeyAsEmailAttachment(User.GetUserId(), keyStore.ToString(), cancellationToken).GetAwaiter().GetResult();
            return new OkResult();
            }
            catch (UserException ex)
            {
                return new ConflictActionResult(ex.Message);
            }
        }

        [Route("ppid/cancel/{ppid}")]
        [HttpPut]
        public IActionResult CancelChangePricePlan([FromRoute] string ppid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ppid))
                return BadRequest();

            try
            {
                _userSubscriptionService.CancelPendingPricePlan(User.GetUserId(), ppid, cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch (SubscriptionException ex)
            {
                return new ConflictActionResult(ex.Message);
            }
        }
    }
}