namespace TreasurehuntApi.Model
{
    public class HtmlPageInputs
    {
        public int UrlDivertTimeInMilliSeconds { get; set; }

        public string? Message { get; set; }
        public string? ImageToShow { get; set; }

        public int HTMLErrorCode { get; set; }

        public HtmlPageInputs()
        {
            HTMLErrorCode = 200;
        }
    }
}
