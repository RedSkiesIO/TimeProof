using AtlasCity.TimeProof.Abstractions.Requests;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using AtlasCity.TimeProof.Api.Extensions;
using AtlasCity.TimeProof.Common.Lib.Exceptions;
using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading;

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
            var userId = User.GetUserId();
            var user = _userService.GetUserById(userId, cancellationToken).GetAwaiter().GetResult();
            var response = user.ToResponse();

            var userKey = _userService.GetUserKey(userId, cancellationToken).GetAwaiter().GetResult();
            if (response != null && userKey != null)
                response.KeyValue = userKey.KeyDetails;

            return new SuccessActionResult(response);
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


        [Route("user")]
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest user, CancellationToken cancellationToken)
        {
            if (user == null)
                return BadRequest();

            try
            {
                _userService.UpdateUser(User.GetUserId(), user.FirstName, user.LastName, cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch (SubscriptionException ex)
            {
                return new ConflictActionResult(ex.Message);
            }

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

        [Route("user/paymentmethod/{pmid}")]
        [HttpPut]
        public IActionResult UpdateCustomerPaymentMethod([FromBody] AddressRequest newAddress, [FromRoute] string pmid, CancellationToken cancellationToken)
        {
            if (newAddress == null)
                return BadRequest();

            try
            {
                _userSubscriptionService.UpdateCustomerPaymentMethod(User.GetUserId(), pmid, newAddress.ToDao(), cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch (SubscriptionException ex)
            {
                return new ConflictActionResult(ex.Message);
            }
        }

        [Route("user/upgrade/{ppid}")]
        [HttpPut]
        public IActionResult ChangePricePlan([FromRoute] string ppid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ppid))
                return BadRequest();

            try
            {
                _userSubscriptionService.ChangePricePlan(User.GetUserId(), ppid, cancellationToken).GetAwaiter().GetResult();
                return new OkResult();
            }
            catch (SubscriptionException ex)
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

            try
            {
                _userService.SendWelcomeEmailAndStoreKey(User.GetUserId(), keyStore.ToString(), cancellationToken).GetAwaiter().GetResult();
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