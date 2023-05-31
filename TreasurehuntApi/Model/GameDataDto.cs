using TreasurehuntApi.Data;

namespace TreasurehuntApi.Model
{
    public class SingleGameFormatDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        // Use to identify the game (It's one character long)
        public string Code { get; set; }

        // This is CSV data
        public List<string> DataHeaders { get; set; }

        public List<string[]> Data { get; set; }

        public SingleGameFormatDto() {
        
            Data = new List<string[]>();
            DataHeaders = new List<string>();
        }
    }

    public class StateRunReturnDto
    {
        public bool IsGameStarted { get; set; }

        public string? UrlToRedirect { get; set; }
        public bool IsSuccessfulScan { get; set; }

        public bool IsCurrentTeamFinished { get; set; }

        // Is game finished by both the team
        public bool IsGameOver { get; set; }

        public string? Error { get; set; }
    }
    public class GameDataDto
    {
        public List<SingleGameFormatDto> AllGames { get; set; }

        public GameDataDto()
        {
            AllGames = new List<SingleGameFormatDto>();
        }

        // Code Index for teamA at 0th column, and 2nd column for teamB
        public static int CodeIndexInRow(string teamName)
        {
            return (UserData.TeamAName == teamName) ? 0 : 2;
        }

        // InstructionUrl Index for teamA at 0th column, and 2nd column for teamB
        public static int InstructionsUrlIndexInRow(string teamName)
        {
            return (UserData.TeamAName == teamName) ? 1 : 3;
        }
    }
}
