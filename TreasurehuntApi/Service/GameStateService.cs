using Microsoft.Extensions.Configuration;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public class GameStateService
    {
        private readonly ILogger<GameStateService> _logger;
        private readonly InMemoryDataService _inMemoryDataService;

        public GameStateService(ILogger<GameStateService> logger,
            InMemoryDataService inMemoryDataService)
        {
            _logger = logger;
            _inMemoryDataService = inMemoryDataService;
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

        public GameStateDto SelectNewGame(SingleGameFormatDto gameData)
        {
            ResetCurrentGameState();
            var gameState = new GameStateDto(gameData);
            UpdateNewGameState(gameState);
            return gameState;
        }
    }
}
