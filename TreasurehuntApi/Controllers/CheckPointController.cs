using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TreasurehuntApi.lib;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckPointsController : BaseController
    {
        private readonly ILogger<CheckPointsController> _logger;

        public CheckPointsController(ILogger<CheckPointsController> logger) : base(logger) 
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{code}")]
        [SwaggerResponse(StatusCodes.Status200OK, "If checkpoint has found", typeof(User))]
        public IActionResult Get([FromRoute]string code)
        {
            var user = AuthLib.GetLoggedInUser(Request);
            if (user == null) { return Unauthorized(); }

            // TODO validate 
            return Redirect("https://drive.google.com/file/d/1uCbt4cRdRsBuu_TsmHWWGstt9RTrCcKe/view");
;
        }
    }
}