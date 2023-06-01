using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TreasurehuntApi.Data;
using TreasurehuntApi.lib;
using TreasurehuntApi.Model;

using System.IO;
using Newtonsoft.Json;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace TreasurehuntApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        public const string LoginCookie = "UserId";

        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration Configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration) : base(logger) 
        {
            _logger = logger;
            Configuration = configuration;
        }

        private string? getEncodedLoginString(UserLoginRequest request)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(request);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                string base64Encoded = Convert.ToBase64String(bytes);
                return base64Encoded;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private UserLoginRequest? getDecodedRequest(string encodedStrng)
        {
            try
            {
                byte[] base64Bytes = Convert.FromBase64String(encodedStrng);
                string jsonString = Encoding.UTF8.GetString(base64Bytes);

                UserLoginRequest? request = JsonConvert.DeserializeObject<UserLoginRequest>(jsonString);
                return request;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void setLoginCookie(UserResponseDto user)
        {
            var maxUserSessionInMinutes = Configuration.GetValue<int>("auth:maxUserSessionInMinutes");
            AuthLib.SetCookie(Response, user.Id.ToString(), maxUserSessionInMinutes);
        }

        [HttpPost]
        [Route("login")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 found the user", typeof(string))]
        public IActionResult Login(UserLoginRequest request)
        {
            var user = UserData.ValidateUser(request);
            if (user == null) { return Unauthorized(); }

            setLoginCookie(user);
            // Generate Encoded string
            return Ok(getEncodedLoginString(request));
        }

        [HttpGet]
        [Route("fastLogin")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns 200 found the user", typeof(UserResponseDto))]
        public IActionResult FastLogin([FromQuery] string loginEncodedString)
        {
            var request =  getDecodedRequest(loginEncodedString);
            if(request == null) { return Unauthorized(); }
            
            var user = UserData.ValidateUser(request);
            if (user == null) { return Unauthorized(); }

            setLoginCookie(user);

            return Ok(user);
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "If user is logged in", typeof(UserResponseDto))]
        public IActionResult Get()
        {
            var user = AuthLib.GetLoggedInUser(Request);
            if (user == null) { return Unauthorized(); }
            return Ok(UserData.GetUserResponse(user));
        }

        [HttpGet]
        [Route("testconfig")]
        [SwaggerResponse(StatusCodes.Status200OK, "Check if configuration are working", typeof(User))]
        public IActionResult TestConfig()
        {
            var maxUserSessionInMinutes = Configuration.GetValue<int>("auth:maxUserSessionInMinutes");
            return Ok(maxUserSessionInMinutes);
        }

        [HttpGet]
        [Route("version")]
        [SwaggerResponse(StatusCodes.Status200OK, "Get version", typeof(string))]
        public IActionResult GetVersion()
        {

            string jsonContent = System.IO.File.ReadAllText("autoversion.json");

            // Parse the JSON content
            VersionDto? versionObj = JsonConvert.DeserializeObject<VersionDto>(jsonContent);

            // Access specific properties in the JSON object
            string? version = versionObj?.Version;

            return Ok(version);
        }
    }
}