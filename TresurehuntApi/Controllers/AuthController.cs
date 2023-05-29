using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TresurehuntApi.Data;
using TresurehuntApi.Model;

namespace TresurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "login")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 found the user", typeof(User))]
        public IActionResult Login(UserLoginRequest request)
        {
            var user = UserData.ValidateUser(request);
            return user == null ? Unauthorized() : Ok(user);
        }
    }
}