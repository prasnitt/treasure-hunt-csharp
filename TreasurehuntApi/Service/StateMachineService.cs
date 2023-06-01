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
        private static int InCorrectGameCodePoint = -2;
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
        public StateRunReturnDto Run(string teamName,string gameCode, int scanCode)
        {
            var ret = new StateRunReturnDto();
            var state = _gameStateService.GetCurrentGameState();

            if (state == null)
            {
                ret.IsGameStarted = false;
                return ret;
            }

            ret.IsGameStarted = true;

            // If Game has over, means both team are finised
            if (state.IsGameOver)
            {
                ret.IsGameOver = true;
                return ret;
            }

            var teamState = state.TeamWiseGameState[teamName];

            // If team has already finished the game ignore anything
            if (teamState.FinishedAt != null)
            {
                ret.IsCurrentTeamFinished = true;
                return ret;
            }

            var gameData = _gameStateService.GetCurrentGameData(state);
            // Scan code will be a decimal number
            var (expectedCode, error) = _gameStateService.GetNextExpectedCode(teamName, state, gameData);

            if (error != null)
            {
                ret.Error = error;
                return ret;
            }

            bool isGameCodeMatch = (gameData.Code == gameCode);

            if (expectedCode == scanCode && isGameCodeMatch)
            {
                // Get Instructions for next checkpoint
                (ret.UrlToRedirect, ret.Error) = _gameStateService.GetInstructionUrl(teamName, state, gameData);

                state = UpdateStateOnSuccessfulCodeMatch(teamName, state);
                ret.IsSuccessfulScan = true;

                // Check if team has finished
                teamState = state.TeamWiseGameState[teamName];
                if (teamState.FinishedAt != null)
                {
                    ret.IsCurrentTeamFinished = true;
                    ret.Error = null;
                }
            }
            else
            {
                state = UpdateStateOnFulureCodeNotMatch(teamName, state, isGameCodeMatch);
                ret.IsSuccessfulScan = false;
            }

            // Update the state
            _gameStateService.UpdateNewGameState(state);

            return ret;
        }

        private GameStateDto UpdateStateOnFulureCodeNotMatch(string teamName, GameStateDto state, bool isGameCodeMatch)
        {
            var teamState = state.TeamWiseGameState[teamName];
            // Penalty points
            if (isGameCodeMatch)
            {
                AddToTeamScore(teamState, InCorrectCodePoint);
            }
            else
            {
                AddToTeamScore(teamState, InCorrectGameCodePoint);
            }

            return state;
        }

        private GameStateDto UpdateStateOnSuccessfulCodeMatch(string teamName, GameStateDto state)
        {
            var teamState = state.TeamWiseGameState[teamName];
            var otherTeamState = state.TeamWiseGameState[GetOtherTeamName(teamName)];

            AddToTeamScore(teamState, CorrectCodePoint);

            // Check if at finished
            if ((teamState.CurCheckPointIndex + 1) == state.TotalNumberOfCheckPoints)
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
                    AddToTeamScore(teamState, WinningEarlyBonusPoint);
                }
            }

            // Increment CheckPoint & point
            teamState.CurCheckPointIndex++;

            state.TeamWiseGameState[teamName] = teamState;
            return state;
        }

        private void AddToTeamScore(TeamWiseGameStateDto teamState, int score)
        {
            teamState.ScoreTransaction[teamState.CurCheckPointIndex].Add(score);
            teamState.CurrentScore += score;
        }


        private string GetOtherTeamName(string teamName)
        {
            return (teamName == UserData.TeamAName) ? UserData.TeamBName : UserData.TeamAName;
        }
    }
}

