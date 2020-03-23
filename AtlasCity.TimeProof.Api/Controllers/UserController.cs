using System.Threading;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Api.ActionResults;
using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AtlasCity.TimeProof.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(userService, nameof(userService)).NotNull();

            _logger = logger;
            _userService = userService;
        }

        [Route("users/{email}")]
        [HttpGet]
        public IActionResult Get([FromRoute] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest();

            return Ok(Json(_userService.GetUserByEmail(email, cancellationToken).GetAwaiter().GetResult()));
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDao user, CancellationToken cancellationToken)
        {
            if (user == null)
                return BadRequest();

            var newUser = _userService.CreateUser(user, cancellationToken).GetAwaiter().GetResult();

            return new CreatedActionResult(newUser);
        }
    }
}