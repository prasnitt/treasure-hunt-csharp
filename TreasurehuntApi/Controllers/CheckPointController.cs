using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TreasurehuntApi.lib;
using TreasurehuntApi.Model;
using TreasurehuntApi.Service;
using static TreasurehuntApi.Data.UserData;

namespace TreasurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckPointsController : BaseController
    {
        private readonly ILogger<CheckPointsController> _logger;
        private readonly StateMachineService _stateMachineService;

        public CheckPointsController(ILogger<CheckPointsController> logger, StateMachineService stateMachineService) : base(logger) 
        {
            _logger = logger;
            _stateMachineService = stateMachineService;
        }

        [HttpGet]
        [Route("{gameCode}/{scannedCode}")]
        [SwaggerResponse(StatusCodes.Status200OK, "If checkpoint has found", typeof(User))]
        public IActionResult Get([FromRoute] string gameCode, [FromRoute]int scannedCode)
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.Team);
            if (user == null) { return Unauthorized(); }

            // Run the state machine
            var (state, error) = _stateMachineService.Run(user.FullName, gameCode, scannedCode);
            if (error != null)
            {
                return StatusCode(500, $"Internal Server Error: `{error}`");
            }

            // TODO Check if match or mismatch

            // TODO check if Game is over

            // TODO check if this user has finished

            // TODO get next url to divert

            //return Redirect("https://drive.google.com/file/d/1uCbt4cRdRsBuu_TsmHWWGstt9RTrCcKe/view");

            return Ok();

        }
    }
}