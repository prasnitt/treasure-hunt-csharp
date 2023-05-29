using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TresurehuntApi.Data;
using TresurehuntApi.lib;
using TresurehuntApi.Model;

namespace TresurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        public const string LoginCookie = "UserId";

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger) : base(logger) 
        {
            _logger = logger;
        }

        [HttpPost(Name = "login")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 found the user", typeof(User))]
        public IActionResult Login(UserLoginRequest request)
        {
            var user = UserData.ValidateUser(request);
            if (user == null) { return Unauthorized(); }
            AuthLib.SetCookie(Response, user.Id.ToString());
            return Ok(user);
        }


        [HttpGet(Name = "get")]
        [SwaggerResponse(StatusCodes.Status200OK, "If user is logged in", typeof(User))]
        public IActionResult Get()
        {
            var user = AuthLib.GetLoggedInUser(Request);
            if (user == null) { return Unauthorized(); }
            return Ok(user);
        }
    }
}