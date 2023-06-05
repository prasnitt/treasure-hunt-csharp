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
            // Add dummy state
            UpdateNewGameState(new GameStateDto());
        }

        public void UpdateNewGameState(GameStateDto gameState)
        {
            gameState.UpdatedAt = DateTimeOffset.UtcNow;

            // Update the team member's name
            foreach (var teamName in UserData.TeamNames)
            {
                var (_, teamMembers) = GetTeamMemberNames(teamName);
                if(teamMembers != null) {
                    gameState.TeamWiseGameState[teamName].TeamMemberNames = teamMembers;
                }
            }
            
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

        public (string?, List<string>?) GetTeamMemberNames(string teamName)
        {
            var teamMembersData = (TeamMembersNameDto)_inMemoryDataService.GetItemFromCache(InMemoryDataService.TeamMemberNamesKey);

            if (teamMembersData == null || !teamMembersData.AllTeamMemberNames.ContainsKey(teamName))
            {
                return ($" Team {teamName} Does not exists", null);
            }

            return (null, teamMembersData.AllTeamMemberNames[teamName]);
        }

        public (string?, TeamMembersNameDto) UpdateTeamMemberNames(string teamName, List<string> teamMemberNames)
        {
            var teamMembersData = (TeamMembersNameDto)_inMemoryDataService.GetItemFromCache(InMemoryDataService.TeamMemberNamesKey);

            if (teamMembersData == null)
            {
                teamMembersData = new TeamMembersNameDto();
            }

            if (!teamMembersData.AllTeamMemberNames.ContainsKey(teamName))
            {
                return ($" Team {teamName} Does not exists", null);
            }


            teamMembersData.AllTeamMemberNames[teamName] = teamMemberNames;
            _inMemoryDataService.AddItemToCache(InMemoryDataService.TeamMemberNamesKey, teamMembersData);

            var gameState = GetCurrentGameState();
            if (gameState != null) {
                UpdateNewGameState(gameState);
            }
            return (null, teamMembersData);
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


        public (string?, string?) GetInstructionUrl(string teamName, GameStateDto state, SingleGameFormatDto? gameData, 
            bool getPrevious = false)
        {
            var (url, error) = GetGameData(teamName, state, gameData, false, getPrevious);
            return (url, error);
        }

        private (string?, string?) GetGameData(string teamName, GameStateDto state, SingleGameFormatDto? gameData, bool isCode,
            bool getPrevious = false)
        {
            if (gameData == null) { return (null, "Game not started"); }

            if (!state.TeamWiseGameState.ContainsKey(teamName))
            {
                return (null, $"TeamName `{teamName}` not found");
            }

            var teamState = state.TeamWiseGameState[teamName];

            if (gameData.Data.Count <= teamState.CurCheckPointIndex)
            {
                return (null, null);
            }

            int checkPointIndex = teamState.CurCheckPointIndex;
            // Get previous checkpoint data
            if (getPrevious)
            {
                if (checkPointIndex == 0)
                {
                    return (null, null);
                }

                // reduce the checkpoint value
                checkPointIndex -= 1;
            }

            // While Calculating Instructions, we will be using next row data
            if (!isCode)
            {
                checkPointIndex += 1;

                // Check if this is the last data point (no next instruction)
                if (gameData.Data.Count <= checkPointIndex)
                {
                    return (null, null);
                }
            }

            var row = gameData.Data[checkPointIndex];

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
