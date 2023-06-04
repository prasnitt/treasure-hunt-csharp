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
        [SwaggerResponse(StatusCodes.Status200OK, "If checkpoint is matched", typeof(ContentResult))]
        public IActionResult Get([FromRoute] string gameCode, [FromRoute]int scannedCode)
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.Team);
            if (user == null) { return HtmlResponseGeneratorService.GetHtmlPage("Unauthorized", "Please contact Admin (Mr. Prashant)"); }

            // Run the state machine
            var stateRunResponse = _stateMachineService.Run(user.FullName, gameCode, scannedCode);
            if (stateRunResponse.Error != null)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("InternalServerError", stateRunResponse.Error);
            }

            // If game has not started
            if (!stateRunResponse.IsGameStarted)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("GameNotStarted", "Please Wait");
            }

            if (stateRunResponse.IsGameOver)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("GameOver");
            }

            // Current team has finished
            if (stateRunResponse.IsCurrentTeamFinished)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("FinishedGame");
            }

            // if scan mismatch
            if (!stateRunResponse.IsSuccessfulScan)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("WrongScan", 
                    urlToDivert : stateRunResponse.UrlToRedirect);
            }

            // redirect to next instruction
            if (stateRunResponse.UrlToRedirect != null)
            {
                return HtmlResponseGeneratorService.GetHtmlPage("SuccessfulScan", 
                    "You are going to see instruction for next Checkpoint",
                    stateRunResponse.UrlToRedirect);
            }
            
            return Ok();

        }
    }
}