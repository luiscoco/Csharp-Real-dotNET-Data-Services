// =============================================================================
// ProcessedPost.cs — A simplified output model used after processing raw posts.
//
// In real-world applications, you often transform API data before saving it.
// This model holds only the fields we care about for the output file:
// the post Id, its Title, and a short Summary derived from the Body.
// =============================================================================

using System.Text.Json.Serialization;

namespace RealDataServiceDemo.Models;

/// <summary>
/// Simplified projection of a Post, used for the final output file.
/// </summary>
public class ProcessedPost
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}
