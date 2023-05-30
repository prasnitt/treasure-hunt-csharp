using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TreasurehuntApi.Service;

public class StartStopService : IHostedService
{
    private readonly GameDataService _gameDataService;

    public StartStopService(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
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
    }
}
