using TreasurehuntApi.Data;

namespace TreasurehuntApi.Model
{
    public class SingleGameFormatDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        // This is CSV data
        public List<string> DataHeaders { get; set; }

        public List<string[]> Data { get; set; }

        public SingleGameFormatDto() {
        
            Data = new List<string[]>();
            DataHeaders = new List<string>();
        }
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
