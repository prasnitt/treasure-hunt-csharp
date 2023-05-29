using Microsoft.AspNetCore.Mvc;
using TresurehuntApi.Data;
using TresurehuntApi.Model;

namespace TresurehuntApi.Controllers
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