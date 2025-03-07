//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Net.Http.Headers;
//using System.Text.Json; 

//namespace EmployeeAdmin2.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class Restaurant : ControllerBase
//    {
//        private readonly HttpClient _httpClient;
//        private const string ThirdPartyApiUrl = "http://192.168.52.38:3000/api/v1/restaurant/login";

//        public Restaurant(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromForm] LoginRequest request)
//        {
//            if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Password))
//            {
//                return BadRequest("Phone and password are required.");
//            }

//            try
//            {
//                // Prepare form-urlencoded data
//                var formData = new Dictionary<string, string>
//                {
//                    { "phone", request.Phone },
//                    { "password", request.Password }
//                };

//                var httpContent = new FormUrlEncodedContent(formData);
//                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

//                HttpResponseMessage response = await _httpClient.PostAsync(ThirdPartyApiUrl, httpContent);

//                string responseData = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    return StatusCode((int)response.StatusCode, $"Error from third-party API: {responseData}");
//                }
//                var re = JsonSerializer.Deserialize<object>(responseData);

//                return Ok(re);
//            }
//            catch (HttpRequestException ex)
//            {
//                return StatusCode(500, $"Internal Server Error: {ex.Message}");
//            }
//        }
//    }

//    public class LoginRequest
//    {
//        public required string Phone { get; set; }
//        public required string Password { get; set; }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Nodes; // For JsonObject, if you wish to use it.

namespace EmployeeAdmin2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Restaurant : ControllerBase
    {
        private readonly ApiWrapper _apiWrapper;
        private const string ThirdPartyApiUrl = "http://192.168.52.38:3000/api/v1/restaurant";

        public Restaurant(HttpClient httpClient)
        {
            _apiWrapper = new ApiWrapper(httpClient);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Phone and password are required.");
            }

            try
            {
                var formData = new Dictionary<string, string>
                {
                    { "phone", request.Phone },
                    { "password", request.Password }
                };

                var response = await _apiWrapper.PostAsync<JsonObject>($"{ThirdPartyApiUrl}/login", formData);

                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetCategories([FromHeader(Name = "Authorization")] string? authorizationHeader, [FromQuery] Dictionary<string, string>? queryParams)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest("Authorization header with Bearer token is required.");
            }

            string bearerToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var response = await _apiWrapper.GetAsync<JsonArray>($"{ThirdPartyApiUrl}/category", bearerToken, queryParams);
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        public class LoginRequest
        {
            public required string Phone { get; set; }
            public required string Password { get; set; }
        }
    }
}