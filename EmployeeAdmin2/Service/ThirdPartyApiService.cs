using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace EmployeeAdmin2.Services
{
    public class ThirdPartyApiService
    {
        private readonly HttpClient _httpClient;

        public ThirdPartyApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, object? Data, string? ErrorMessage)> CallApiAsync(
            string url,
            HttpMethod method,
            object? requestBody = null,
            bool sendAsFormUrlEncoded = false)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                // Handle form-urlencoded content
                if (requestBody != null)
                {
                    if (sendAsFormUrlEncoded)
                    {
                        var keyValues = new Dictionary<string, string>();
                        foreach (var prop in requestBody.GetType().GetProperties())
                        {
                            keyValues.Add(prop.Name, prop.GetValue(requestBody)?.ToString() ?? "");
                        }

                        request.Content = new FormUrlEncodedContent(keyValues);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    }
                    else
                    {
                        string jsonContent = JsonSerializer.Serialize(requestBody);
                        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    }
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, $"Error: {response.StatusCode} - {responseData}");
                }

                var jsonResponse = JsonSerializer.Deserialize<object>(responseData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return (true, jsonResponse, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, null, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
