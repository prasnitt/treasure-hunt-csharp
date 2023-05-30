using Microsoft.Extensions.Configuration;
using TreasurehuntApi.Data;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public class GameStateService
    {
        private readonly ILogger<GameStateService> _logger;
        private readonly InMemoryDataService _inMemoryDataService;
        private readonly GameDataService _gameDataService;


        public GameStateService(ILogger<GameStateService> logger,
            InMemoryDataService inMemoryDataService,
            GameDataService gameDataService)
        {
            _logger = logger;
            _inMemoryDataService = inMemoryDataService;
            _gameDataService = gameDataService;
        }

        public void ResetCurrentGameState()
        {
            _inMemoryDataService.RemoveItemFromCache(InMemoryDataService.GameStateKey);
        }

        public void UpdateNewGameState(GameStateDto gameState)
        {
            _inMemoryDataService.AddItemToCache(InMemoryDataService.GameStateKey, gameState);
        }

        public GameStateDto GetCurrentGameState()
        {
            return (GameStateDto)_inMemoryDataService.GetItemFromCache(InMemoryDataService.GameStateKey);
        }

        public SingleGameFormatDto? GetCurrentGameData(GameStateDto state = null)
        {
            if (state == null) { state = GetCurrentGameState(); }
            if (state == null) { return null; }

            return _gameDataService.GetGameDataByCode(state.GameCode);
        }

        public GameStateDto SelectNewGame(SingleGameFormatDto gameData)
        {
            ResetCurrentGameState();
            var gameState = new GameStateDto(gameData);
            UpdateNewGameState(gameState);
            return gameState;
        }

        public (int, string?) GetNextExpectedCode(string teamName, GameStateDto state, SingleGameFormatDto gameData)
        {
            if (gameData == null) { return (-1, "Game not started"); }

            if (!state.TeamWiseGameState.ContainsKey(teamName))
            {
                return (-1, $"TeamName `{teamName}` not found");
            }

            var teamState = state.TeamWiseGameState[teamName];

            if (gameData.Data.Count <= teamState.CurCheckPointNum)
            {
                return (-1, $"For Team `{teamName}` Check point `{teamState.CurCheckPointNum}` bigger than expected `{gameData.Data.Count-1}`");
            }

            var row = gameData.Data[teamState.CurCheckPointNum];
            var codeIndex = GameDataDto.CodeIndexInRow(teamName);
            if (row.Length <= codeIndex)
            {
                return (-1, $"For Team `{teamName}` Cde Index `{codeIndex}` bigger than expected `{row.Length - 1}`");
            }

            int code = int.Parse(row[codeIndex]);

            return (code, null);
        }
    }
}
