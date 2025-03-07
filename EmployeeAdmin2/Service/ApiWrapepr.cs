using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

public class ApiWrapper
{
    private readonly HttpClient _httpClient;

    public ApiWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> PostAsync<T>(string url, Dictionary<string, string> formData)
    {
        var httpContent = new FormUrlEncodedContent(formData);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode(); // Throws exception for non-success status

        string responseData = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseData);
    }

    public async Task<T> GetAsync<T>(string url, string bearerToken, Dictionary<string, string>? queryParams = null)
    {
        if (string.IsNullOrEmpty(bearerToken))
        {
            throw new ArgumentException("Bearer token is required.", nameof(bearerToken));
        }

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }
        }

        uriBuilder.Query = query.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseData = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseData);
    }
}