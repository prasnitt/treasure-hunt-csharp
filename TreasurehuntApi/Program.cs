using TreasurehuntApi.Service;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigureServices(builder.Services);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddMemoryCache();

    services.AddTransient<InMemoryDataService>();
    
    services.AddSingleton<InMemoryDataService>();
    services.AddSingleton<GameDataService>();
    services.AddSingleton<GameStateService>();
    
    services.AddHostedService<StartStopService>();
}