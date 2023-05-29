using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TreasurehuntApi.Data;
using TreasurehuntApi.lib;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        public const string LoginCookie = "UserId";

        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration Configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration) : base(logger) 
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 found the user", typeof(User))]
        public IActionResult Login(UserLoginRequest request)
        {
            var maxUserSessionInMinutes = Configuration.GetValue<int>("auth:maxUserSessionInMinutes");

            var user = UserData.ValidateUser(request);
            if (user == null) { return Unauthorized(); }
            AuthLib.SetCookie(Response, user.Id.ToString(), maxUserSessionInMinutes);
            return Ok(user);
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "If user is logged in", typeof(User))]
        public IActionResult Get()
        {
            var user = AuthLib.GetLoggedInUser(Request);
            if (user == null) { return Unauthorized(); }
            return Ok(user);
        }

        [HttpGet]
        [Route("testconfig")]
        [SwaggerResponse(StatusCodes.Status200OK, "If user is logged in", typeof(User))]
        public IActionResult TestConfig()
        {
            var maxUserSessionInMinutes = Configuration.GetValue<int>("auth:maxUserSessionInMinutes");
            return Ok(maxUserSessionInMinutes);
        }
    }
}