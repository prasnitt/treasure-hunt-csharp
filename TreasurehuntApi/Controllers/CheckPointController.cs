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
            var stateRunResponse = _stateMachineService.Run(user.FullName, gameCode, scannedCode);
            if (stateRunResponse.Error != null)
            {
                return StatusCode(500, $"Internal Server Error: `{stateRunResponse.Error}`");
            }

            // If game has not started
            if (!stateRunResponse.IsGameStarted)
            {
                // TODO redirect to url
                return StatusCode(400, $"Game has not started");
            }

            if (stateRunResponse.IsGameOver)
            {
                // TODO redirect to url
                return StatusCode(200, $"Game has been finished");
            }

            // Current team has finished
            if (stateRunResponse.IsCurrentTeamFinished)
            {
                // TODO redirect to url
                return StatusCode(200, $"Current team finished");
            }


            // TODO Check if match or mismatch
            if (!stateRunResponse.IsSuccessfulScan)
            {
                // TODO redirect to url
                return StatusCode(400, $"Invalid Scan");
            }

            // redirect to next instruction
            if (stateRunResponse.UrlToRedirect != null)
            {
                return Redirect(stateRunResponse.UrlToRedirect);
            }
            
            return Ok();

        }
    }
}