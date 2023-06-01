using TreasurehuntApi.Data;

namespace TreasurehuntApi.Model
{
    
    public class TeamWiseGameStateDto
    {
        const int startingScore = 20;

        public int CurrentScore { get; set; }

        public DateTimeOffset? FinishedAt { get; set; }

        public int CurCheckPointIndex { get; set; }

        public List<List<int>>  ScoreTransaction { get; set; }

        public TeamWiseGameStateDto(int totalNumberOfCheckPoints) 
        {
            ScoreTransaction = new List<List<int>>();

            for (int i = 0; i < totalNumberOfCheckPoints; i++)
            {
                ScoreTransaction.Add(new List<int>());
            }

            CurrentScore = startingScore;
            ScoreTransaction[0].Add(CurrentScore);
        }

    }

    public class GameStateDto
    {
        public Guid Id { get; set; }
        public Guid GameDataId { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string GameName { get; set; }
        public string GameCode { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        // If both team finished
        public bool IsGameOver { get; set; }

        public int TotalNumberOfCheckPoints { get; set; }

        public Dictionary<String, TeamWiseGameStateDto> TeamWiseGameState { get; set; }

        public GameStateDto(SingleGameFormatDto gameData)
        {
            Id = Guid.NewGuid();

            GameDataId = gameData.Id;
            GameName = gameData.Name;
            GameCode = gameData.Code;
            StartedAt = DateTimeOffset.UtcNow;
            TotalNumberOfCheckPoints = gameData.Data.Count;

            // Init team scores
            TeamWiseGameState = new Dictionary<String, TeamWiseGameStateDto>();
            TeamWiseGameState[UserData.TeamAName] = new TeamWiseGameStateDto(TotalNumberOfCheckPoints);
            TeamWiseGameState[UserData.TeamBName] = new TeamWiseGameStateDto(TotalNumberOfCheckPoints);
        }
    }
}
