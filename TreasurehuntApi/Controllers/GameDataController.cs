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
    public class GameDataController : BaseController
    {
        private readonly GameDataService _gameDataService;
        private readonly ILogger<GameDataController> _logger;

        public GameDataController(ILogger<GameDataController> logger, GameDataService gameDataService) : base(logger) 
        {
            _logger = logger;
            _gameDataService = gameDataService;
        }

        [HttpGet]
        [Route("LoadFromSheet")]
        [SwaggerResponse(StatusCodes.Status200OK, "Load all games data from google sheet", typeof(GameDataDto))]
        public IActionResult LoadFromSheet()
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);

            if (user == null) { return Unauthorized(); }

            var gameData = _gameDataService.LoadAllGamesData();

            return Ok(gameData);
;
        }

        [HttpGet]
        [Route("LoadFromInMemory")]
        [SwaggerResponse(StatusCodes.Status200OK, "Load all games data from in memory", typeof(GameDataDto))]
        public IActionResult LoadFromInMemory()
        {
            var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);

            if (user == null) { return Unauthorized(); }

            var gameData = _gameDataService.GetGameData();

            return Ok(gameData);
            ;
        }

        [HttpGet]
        [Route("pdfData")]
        [SwaggerResponse(StatusCodes.Status200OK, "Load all games data from in memory", typeof(QrCodePdf))]
        public IActionResult GetPdfData([FromQuery] string gameCode, [FromQuery] int numberUntil)
        {
            //var user = AuthLib.GetLoggedInUser(Request, UserRoles.SuperAdmin);

            //if (user == null) { return Unauthorized(); }
            
            var qrCodePdfData = QRCodeGeneratorService.GetQrCodesPdfData(gameCode, numberUntil);

            return Ok(qrCodePdfData);
            ;
        }
    }
}