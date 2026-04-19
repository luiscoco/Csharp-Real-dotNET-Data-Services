// =============================================================================
// ApiService.cs — Handles HTTP communication with the remote API.
//
// This service:
//   1. Creates an HttpClient to make GET requests.
//   2. Calls the configured API endpoint asynchronously.
//   3. Validates the HTTP response (throws on failure).
//   4. Deserializes the JSON body into a List<Post>.
//
// In production apps, you would typically register HttpClient via
// IHttpClientFactory for connection pooling. For this learning demo,
// we keep it simple with a direct HttpClient instance.
// =============================================================================

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RealDataServiceDemo.Models;

namespace RealDataServiceDemo.Services;

/// <summary>
/// Fetches posts from a remote JSON API.
/// </summary>
public class ApiService
{
    private readonly ILogger<ApiService> _logger;

    public ApiService(ILogger<ApiService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calls the given URL and returns a list of <see cref="Post"/> objects.
    /// </summary>
    /// <param name="requestUrl">The full URL to call (e.g. https://jsonplaceholder.typicode.com/posts).</param>
    /// <returns>A list of deserialized Post objects.</returns>
    public async Task<List<Post>> GetPostsAsync(string requestUrl)
    {
        _logger.LogInformation("Sending GET request to {Url}", requestUrl);

        using var httpClient = new HttpClient();

        // Make the HTTP GET request
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        // Validate the response — this throws an exception if status code is not 2xx
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Received HTTP {StatusCode} from API", (int)response.StatusCode);

        // Deserialize the JSON response body into a strongly typed list
        List<Post>? posts = await response.Content.ReadFromJsonAsync<List<Post>>();

        if (posts is null || posts.Count == 0)
        {
            _logger.LogWarning("API returned an empty or null response");
            return [];
        }

        _logger.LogInformation("Successfully deserialized {Count} posts from API", posts.Count);
        return posts;
    }
}
