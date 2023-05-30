using TreasurehuntApi.Data;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public class StateMachineService
    {
        private readonly ILogger<GameStateService> _logger;
        private readonly GameStateService _gameStateService;

        private static int CorrectCodePoint = 10;
        private static int InCorrectCodePoint = -1;
        private static int WinningEarlyBonusPoint = 2;

        public StateMachineService(ILogger<GameStateService> logger,
            GameStateService gameStateService)
        {
            _logger = logger;
            _gameStateService = gameStateService;
        }

        // This state machine will run every time if a code will be scan by a team.
        // It will check if code is correct. 
        // If Yes:
        //      1. Update Point +10
        //      2. Check if Game has finished by the team first
        //      3. Update state to move next
        // If No:
        //      1. Update Point 
        //         1.a. If the last code within 2 minutes. No update
        //         1.b. Else -1
        //      2. Keep for same state
        public (GameStateDto, string?) Run(string teamName, int scanCode)
        {
            var state = _gameStateService.GetCurrentGameState();

            var teamState = state.TeamWiseGameState[teamName];

            // If team has already finished the game ignore anything
            if (teamState.FinishedAt != null)
            {
                return (state, null);
            }


            // Scan code will be a decimal number
            var (expectedCode, error) = _gameStateService.GetNextExpectedCode(teamName, state);

            if (error != null)
            {
                return (null, error);
            }
            

            if (expectedCode == scanCode)
            {
                state = UpdateStateOnSuccessfulCodeMatch(teamName, state);
            }
            else
            {
                state = UpdateStateOnFulureCodeNotMatch(teamName, state);
            }

            // Update the state
            _gameStateService.UpdateNewGameState(state);

            return (state, null);

        }

        private GameStateDto UpdateStateOnFulureCodeNotMatch(string teamName, GameStateDto state)
        {
            var teamState = state.TeamWiseGameState[teamName];
            // Penalty point
            teamState.CurrentScore += InCorrectCodePoint;

            return state;
        }

        private GameStateDto UpdateStateOnSuccessfulCodeMatch(string teamName, GameStateDto state)
        {
            var teamState = state.TeamWiseGameState[teamName];
            var otherTeamState = state.TeamWiseGameState[GetOtherTeamName(teamName)];

            // Increment CheckPoint & point
            teamState.CurCheckPointNum++;
            teamState.CurrentScore += CorrectCodePoint;

            // Check if at finished
            if (teamState.CurCheckPointNum == state.TotalNumberOfCheckPoints)
            {
                teamState.FinishedAt = DateTimeOffset.UtcNow;

                // If other team already finished the game
                if (otherTeamState.FinishedAt != null)
                {
                    state.IsGameOver = true;
                }
                // Give some more bonus point to current team if other team has not finished yet
                else
                {
                    teamState.CurrentScore += WinningEarlyBonusPoint;
                }
            }

            state.TeamWiseGameState[teamName] = teamState;
            return state;
        }

        
        private string GetOtherTeamName(string teamName)
        {
            return (teamName == UserData.TeamAName) ? UserData.TeamBName : UserData.TeamAName;
        }
    }
}

