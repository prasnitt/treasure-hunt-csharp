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

            if (gameState == null)
            {
                return NotFound($"Game has not started");
            }

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
    }
}