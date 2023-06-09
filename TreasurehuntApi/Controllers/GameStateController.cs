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
    public class GameStateController : BaseController
    {
        private readonly ILogger<GameStateController> _logger;
        private readonly GameStateService _gameStateService;
        private readonly GameDataService _gameDataService;

        public GameStateController(ILogger<GameStateController> logger,
            GameStateService gameStateService,
            GameDataService gameDataService) : base(logger) 
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _gameStateService = gameStateService;
        }

        [HttpGet]
        [Route("Get")]
        [SwaggerResponse(StatusCodes.Status200OK, "Get State", typeof(GameStateDto))]
        public IActionResult Get()
        {
            var gameState = _gameStateService.GetCurrentGameState();
            return Ok(gameState);
        }


        [HttpPost]
        [Route("SetNewGame/{gameCode}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Get State", typeof(GameStateDto))]
        public IActionResult SetNewGame([FromRoute] string gameCode)
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);
            if (user == null) { return Unauthorized(); }

            var curGame = _gameDataService.GetGameDataByCode(gameCode);

            if (curGame == null)
            {
                return NotFound($"GameCode : `{gameCode}` not found");
            }

            // Select new Game
            var gameState = _gameStateService.SelectNewGame(curGame);
            return Ok(gameState);
        }

        [HttpPost]
        [Route("reset")]
        [SwaggerResponse(StatusCodes.Status200OK, "Get State", typeof(GameStateDto))]
        public IActionResult Reset()
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);
            if (user == null) { return Unauthorized(); }

            _gameStateService.ResetCurrentGameState();
            return Ok();
        }


        [HttpPost]
        [Route("TeamMemberNames")]
        [SwaggerResponse(StatusCodes.Status200OK, "Is successful", typeof(TeamMembersNameDto))]
        public IActionResult PostKidsName([FromQuery] string teamName, [FromBody] List<string> teamMemberNames)
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);

            if (user == null) { return Unauthorized(); }

            var (error, teamMembersData) = _gameStateService.UpdateTeamMemberNames(teamName, teamMemberNames);
            if (error != null ) { return BadRequest(error); }
            return Ok(teamMembersData);
        }
    }
}