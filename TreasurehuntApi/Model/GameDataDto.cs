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
    }
}
