using TreasurehuntApi.Data;

namespace TreasurehuntApi.Model
{
    public class TeamWiseGameStateDto
    {
        public int CurrentScore { get; set; }

        public DateTimeOffset FinishedAt { get; set; }

        public int CurCheckPointNum { get; set; }

    }

    public class GameStateDto
    {
        public Guid Id { get; set; }
        public Guid GameDataId { get; set; }

        public string GameName { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        public int TotalNumberOfCheckPoints { get; set; }

        public Dictionary<String, TeamWiseGameStateDto> TeamWiseGameState { get; set; }

        public GameStateDto(SingleGameFormatDto gameData)
        {
            Id = Guid.NewGuid();

            GameDataId = gameData.Id;
            GameName = gameData.Name;
            StartedAt = DateTimeOffset.UtcNow;
            TotalNumberOfCheckPoints = gameData.Data.Count;

            // Init team scores
            TeamWiseGameState = new Dictionary<String, TeamWiseGameStateDto>();
            TeamWiseGameState[UserData.teamAName] = new TeamWiseGameStateDto();
            TeamWiseGameState[UserData.teamBName] = new TeamWiseGameStateDto();
        }
    }
}
