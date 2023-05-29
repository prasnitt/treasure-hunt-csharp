using Microsoft.AspNetCore.Mvc;
using TreasurehuntApi.Data;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Controllers
{
    public class BaseController : ControllerBase
    {
        
        private readonly ILogger<BaseController> _logger;

        
        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }


    }
}