using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;
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
            gameState.UpdatedAt = DateTimeOffset.UtcNow;
            _inMemoryDataService.AddItemToCache(InMemoryDataService.GameStateKey, gameState);
        }

        public GameStateDto? GetCurrentGameState()
        {
            return (GameStateDto)_inMemoryDataService.GetItemFromCache(InMemoryDataService.GameStateKey);
        }

        public SingleGameFormatDto? GetCurrentGameData(GameStateDto? state = null)
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

        public (int, string?) GetNextExpectedCode(string teamName, GameStateDto state, SingleGameFormatDto? gameData)
        {
            var (dataValue, error) = GetGameData(teamName, state, gameData, true);

            if (error != null || dataValue == null)
            {
                return (-1, error);
            }

            int code = int.Parse(dataValue);

            return (code, null);
        }


        public (string?, string?) GetInstructionUrl(string teamName, GameStateDto state, SingleGameFormatDto? gameData)
        {
            var (url, error) = GetGameData(teamName, state, gameData, false);
            return (url, error);
        }

        private (string?, string?) GetGameData(string teamName, GameStateDto state, SingleGameFormatDto? gameData, bool isCode)
        {
            if (gameData == null) { return (null, "Game not started"); }

            if (!state.TeamWiseGameState.ContainsKey(teamName))
            {
                return (null, $"TeamName `{teamName}` not found");
            }

            var teamState = state.TeamWiseGameState[teamName];

            if (gameData.Data.Count <= teamState.CurCheckPointIndex)
            {
                return (null, $"For Team `{teamName}` Check point `{teamState.CurCheckPointIndex}` bigger than expected `{gameData.Data.Count - 1}`");
            }

            var row = gameData.Data[teamState.CurCheckPointIndex];

            int columnIndex;
            if (isCode)
                columnIndex = GameDataDto.CodeIndexInRow(teamName);
            else
                columnIndex = GameDataDto.InstructionsUrlIndexInRow(teamName);

            if (row.Length <= columnIndex)
            {
                return (null, $"For Team `{teamName}` data column Index `{columnIndex}` bigger than expected `{row.Length - 1}`");
            }

            return (row[columnIndex], null);
        }
    }
}
