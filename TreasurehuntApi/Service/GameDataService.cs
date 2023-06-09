﻿using TreasurehuntApi.Controllers;
using TreasurehuntApi.lib;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public class GameDataService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<GameDataService> _logger;
        private readonly InMemoryDataService _inMemoryDataService;

        public GameDataService(ILogger<GameDataService> logger, IConfiguration configuration, InMemoryDataService inMemoryDataService)
        {
            _logger = logger;
            Configuration = configuration;
            _inMemoryDataService = inMemoryDataService;
        }

        public GameDataDto LoadAllGamesData()
        {
            var spredSheetId = Configuration.GetValue<string>("googleSheetGameData:spreadsheetId");
            var demoSheetId = Configuration.GetValue<string>("googleSheetGameData:demoSheetId");
            var mediumSheetId = Configuration.GetValue<string>("googleSheetGameData:mediumSheetId");
            var hardSheetId = Configuration.GetValue<string>("googleSheetGameData:hardSheetId");

            var gameData = new GameDataDto();

            gameData.AllGames.Add(loadOneGameData(spredSheetId, demoSheetId, "Demo", "D"));
            gameData.AllGames.Add(loadOneGameData(spredSheetId, mediumSheetId, "Easy", "M"));
            gameData.AllGames.Add(loadOneGameData(spredSheetId, hardSheetId, "Expert", "H"));


            _inMemoryDataService.AddItemToCache(InMemoryDataService.GameDataKey, gameData);

            return gameData;
        }

        public GameDataDto GetGameData()
        {
            var gameData = (GameDataDto)_inMemoryDataService.GetItemFromCache(InMemoryDataService.GameDataKey);
            return gameData;
        }

        public SingleGameFormatDto? GetGameDataByCode(string code)
        {
            var gameData = GetGameData();
            if (gameData == null) { return null; }

            var game = gameData.AllGames.FirstOrDefault(g => g.Code == code);
            return game;
        }

        private SingleGameFormatDto loadOneGameData(string spreadsheetId, string sheetId, string gameName, string code)
        {
            var singleGameData = new SingleGameFormatDto(gameName, code);

            (singleGameData.DataHeaders, singleGameData.Data) = GoogleSpreadSheet.GetSheetAsCsvData(spreadsheetId, sheetId);

            return singleGameData;
        }
    }
}
