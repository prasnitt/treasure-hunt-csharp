using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Rtp.Web.Api.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnv"></param>
        public IndexController(IConfiguration configuration, IWebHostEnvironment hostingEnv)
        {
            _configuration = configuration;
            _env = hostingEnv;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/swagger/index.html");
        }
    }
}
