using Microsoft.AspNetCore.Mvc;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public static class HtmlResponseGeneratorService
    {

        public static Dictionary<string, HtmlPageInputs> htmlTemplates = new Dictionary<string, HtmlPageInputs>()
        {
            { 
                "Unauthorized", 
                new HtmlPageInputs{ 
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/401-error.jpg",
                    Message = "Unauthorized",
                    HTMLErrorCode = 401,
                } 
            },
            {
                "InternalServerError",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/500-error.jpg",
                    Message = "Internal Server Error",
                    HTMLErrorCode = 500,
                }
            },
            {
                "GameNotStarted",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/game-not-started.jpg",
                     Message = "Game Not Started",
                     HTMLErrorCode = 400,
                }
            },
            {
                "GameOver",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/gameOver.png",
                     Message = "Game Over",
                }
            },
            {
                "FinishedGame",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/finishedGame.jpeg",
                     Message = "You have reached your final destination :)",
                }
            },
            {
                "WrongScan",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/WrongQRCode.jpg",
                     Message = "Wrong QR Code Scan :(",
                     HTMLErrorCode = 400,
                }
            },
            {
                "SuccessfulScan",
                new HtmlPageInputs{
                    ImageToShow = "https://purple-glacier-016035500.3.azurestaticapps.net/assets/img/success_scan.gif",
                     Message = "Great, you reached next level :)",
                     HTMLErrorCode = 200,
                     UrlDivertTimeInMilliSeconds = 3000,
                }
            },
        };


        public static ContentResult GetHtmlPage(string templateKey, string? message = null, string? urlToDivert = null)
        {
            HtmlPageInputs? template =  htmlTemplates.ContainsKey(templateKey) ? htmlTemplates[templateKey] : null;

            string htmlContent = "<html> <title>Treasure Hunt</title><head>";

            htmlContent += @"
                    <style>
                        img {
                            width: 100%;
                            height: auto;
                        }
                    </style>
            ";


            if (urlToDivert != null)
            {
                htmlContent += $@"
                      <script>
                        // Function to redirect after a specified time
                        function redirect() {{
                          window.location.href = '{urlToDivert}';
                        }}

                        // Delay the redirect by 5 seconds (5000 milliseconds)
                        setTimeout(redirect, {template?.UrlDivertTimeInMilliSeconds ?? 3000});
                      </script>
                ";
            }


            htmlContent += $@" </head><body><h1>{template?.Message}-{message}</h1>";

            if(template?.ImageToShow != null)
            {
                htmlContent += $"<img src='{template?.ImageToShow}'>";
            }

            htmlContent += "</body></html>";


            return new ContentResult
            {
                Content = htmlContent,
                ContentType = "text/html",
                StatusCode = template?.HTMLErrorCode ?? 200,
            };
        }


    }
}
