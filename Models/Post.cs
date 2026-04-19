// =============================================================================
// Post.cs — Represents a single post from the JSONPlaceholder API.
//
// This model maps directly to the JSON structure returned by the API.
// System.Text.Json uses the [JsonPropertyName] attribute to match JSON keys
// (which are camelCase) to C# properties (which are PascalCase).
// =============================================================================

using System.Text.Json.Serialization;

namespace RealDataServiceDemo.Models;

/// <summary>
/// Strongly typed model for a post from JSONPlaceholder.
/// Each property maps to a field in the API's JSON response.
/// </summary>
public class Post
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}
