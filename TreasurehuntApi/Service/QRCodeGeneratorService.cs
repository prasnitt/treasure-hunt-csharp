using Newtonsoft.Json.Linq;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public static class QRCodeGeneratorService
    {
        public const string QrCodeUrl = "https://pras-th-api.azurewebsites.net/CheckPoints";
        public const int BinaryStringSize = 4;
        public const int QRCodeLength = 30;

        public const int PageMargin = 5;
        public const int OneSquareLength = 40;
        public const int FontSize = 8;
        public const int A4PageWidth = 210;
        public const int A4PageHeight = 297;
        
        // It seems in real world everything need to be multiplied 4 times to take print
        public const int RealWordlIncrementFactor = 4;
        

        public static QrCodePdf GetQrCodesPdfData(string gameCode, int numberUntil)
        {
            var stickerPdf = new QrCodeStickerPdfInputData()
            {
                // A4 sheet size
                PageHeight = A4PageHeight,
                PageWidth = A4PageWidth,
            };

            (stickerPdf.PdfStrings, stickerPdf.QrCodes) = GetPdfData(gameCode, numberUntil);

            considerRealWorldPrintoutFactor(stickerPdf);

            var qrCodePdf = new QrCodePdf() { StickerPdf = stickerPdf };
            return qrCodePdf;
        }

        private static void considerRealWorldPrintoutFactor(QrCodeStickerPdfInputData stickerPdf)
        {
            stickerPdf.PageWidth *= RealWordlIncrementFactor;
            stickerPdf.PageHeight *= RealWordlIncrementFactor;

            foreach(var qr in stickerPdf.QrCodes)
            {
                qr.X *= RealWordlIncrementFactor;
                qr.Y *= RealWordlIncrementFactor;
                
                qr.Width *= RealWordlIncrementFactor;
                qr.Height *= RealWordlIncrementFactor;
            }

            foreach (var str in stickerPdf.PdfStrings)
            {
                str.X *= RealWordlIncrementFactor;
                str.Y *= RealWordlIncrementFactor;
                str.FontSize *= RealWordlIncrementFactor;
            }
        }


        private static (List<PdfString>, List<QrCode>) GetPdfData(string gameCode, int numberUntil)
        {
            var pdfStrings = new List<PdfString>();
            var qrCodes = new List<QrCode>();

            int startX = PageMargin; int startY = PageMargin;
            int pageNumber = 1;
            for (int i=1; i<=numberUntil; i++)
            {
                // QR Code
                var qrCode = new QrCode() { 
                    Height = QRCodeLength , Width = QRCodeLength, PageNumber = pageNumber,
                    X = startX + 5,
                    Y = startY + 1
                };
                qrCode.Value = $"{QrCodeUrl}/{gameCode}/{i}";
                qrCodes.Add(qrCode);

                // Code String
                var pdfString = new PdfString() { 
                    PageNumber = pageNumber,
                    X = startX + 7,
                    Y = startY + 2 + QRCodeLength,
                    FontSize = FontSize
                };
                pdfString.Value = $"{gameCode}-{GetBinaryStringFromNumber(i, BinaryStringSize)}";
                pdfStrings.Add(pdfString);

                // Add string to guideline for cutting the paper
                // Bottom left
                pdfString = new PdfString()
                {
                    PageNumber = pageNumber,
                    X = startX,
                    Y = startY,
                    FontSize = 2,
                    Value = "+",
                };
                pdfStrings.Add(pdfString);

                // Bottom right
                pdfString = new PdfString()
                {
                    PageNumber = pageNumber,
                    X = startX + OneSquareLength,
                    Y = startY,
                    FontSize = 2,
                    Value = "+",
                };
                pdfStrings.Add(pdfString);

                // Top right
                pdfString = new PdfString()
                {
                    PageNumber = pageNumber,
                    X = startX + OneSquareLength,
                    Y = startY + OneSquareLength,
                    FontSize = 2,
                    Value = "+",
                };
                pdfStrings.Add(pdfString);

                // Top left
                pdfString = new PdfString()
                {
                    PageNumber = pageNumber,
                    X = startX,
                    Y = startY + OneSquareLength,
                    FontSize = 2,
                    Value = "+",
                };
                pdfStrings.Add(pdfString);

                // Calculate next square
                startX += OneSquareLength;
                if (A4PageWidth <= (startX + OneSquareLength))
                {
                    startX = PageMargin;
                    startY += OneSquareLength;
                }

                if (A4PageHeight <= (startY + OneSquareLength))
                {
                    startX = PageMargin;
                    startY = PageMargin;
                    pageNumber++;
                }
            }

            return (pdfStrings, qrCodes);
        }


        private static string GetBinaryStringFromNumber(int num, int minStringLen = 4)
        {
            string binary = Convert.ToString(num, 2);
            binary = binary.PadLeft(minStringLen, '0');

            return binary;
        }
    }
}
