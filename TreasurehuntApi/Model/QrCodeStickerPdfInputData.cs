using Newtonsoft.Json;

namespace TreasurehuntApi.Model
{
    public class PdfString
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("fontSize")]
        public int FontSize { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }
    }

    public class QrCode
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }
    }

    public class QrCodeStickerPdfInputData
    {
        [JsonProperty("pdfStrings")]
        public List<PdfString> PdfStrings { get; set; }

        [JsonProperty("qrCodes")]
        public List<QrCode> QrCodes { get; set; }

        [JsonProperty("pageWidth")]
        public int PageWidth { get; set; }

        [JsonProperty("pageHeight")]
        public int PageHeight { get; set; }

        public QrCodeStickerPdfInputData()
        {
            PdfStrings = new List<PdfString>();
            QrCodes = new List<QrCode>();
        }
    }

    public class QrCodePdf
    {
        [JsonProperty("stickerPdf")]
        public QrCodeStickerPdfInputData StickerPdf { get; set; }

    }
}
