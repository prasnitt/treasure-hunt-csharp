using TreasurehuntApi.Service;

public class StartStopService : IHostedService
{
    private readonly GameDataService _gameDataService;
    private readonly GameStateService _gameStateService;

    public StartStopService(GameDataService gameDataService, GameStateService gameStateService)
    {
        _gameDataService = gameDataService;
        _gameStateService = gameStateService;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {

        // Perform initialization logic here
        Initialize();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Perform cleanup logic here
        return Task.CompletedTask;
    }

    private void Initialize()
    {
        // Call your service function here
        // This function will be executed during the initialization of the API

        _gameDataService.LoadAllGamesData();
        _gameStateService.ResetCurrentGameState();
    }
}
