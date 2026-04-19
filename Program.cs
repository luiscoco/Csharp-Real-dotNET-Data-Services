// =============================================================================
// Program.cs — Entry point for the RealDataServiceDemo console application.
//
// This file orchestrates the full workflow:
//   1. Build configuration from appsettings.json
//   2. Set up structured console logging
//   3. Read settings (API URL, output path) from configuration
//   4. Call the remote API to fetch posts
//   5. Process the data (take first 5, project into a simpler model)
//   6. Save the processed results to a local JSON file
//   7. Handle and log any errors that occur
//
// This is the "glue" that connects configuration, services, and logic.
// =============================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealDataServiceDemo.Models;
using RealDataServiceDemo.Services;

// ─── Step 1: Build Configuration ────────────────────────────────────────────
// IConfiguration reads key-value pairs from appsettings.json.
// This is the same pattern used in ASP.NET Core, so learning it here
// prepares you for web development too.

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

// ─── Step 2: Set Up Logging ─────────────────────────────────────────────────
// ILoggerFactory creates loggers that output structured messages to the console.
// Each logger is tagged with a category name (usually the class name) so you
// can tell which part of the app produced each log line.

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConfiguration(configuration.GetSection("Logging"))
        .AddConsole();
});

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

// ─── Step 3: Read Configuration Values ──────────────────────────────────────
string baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://jsonplaceholder.typicode.com";
string endpoint = configuration["ApiSettings:PostsEndpoint"] ?? "/posts";
string outputPath = configuration["OutputSettings:OutputFilePath"] ?? "Output/processed_posts.json";

string fullApiUrl = $"{baseUrl}{endpoint}";

logger.LogInformation("========================================");
logger.LogInformation("  RealDataServiceDemo — Starting");
logger.LogInformation("========================================");
logger.LogInformation("API URL      : {Url}", fullApiUrl);
logger.LogInformation("Output Path  : {OutputPath}", outputPath);

try
{
    // ─── Step 4: Fetch Data from API ────────────────────────────────────
    var apiService = new ApiService(loggerFactory.CreateLogger<ApiService>());
    List<Post> allPosts = await apiService.GetPostsAsync(fullApiUrl);

    logger.LogInformation("Total posts retrieved: {Count}", allPosts.Count);

    // ─── Step 5: Process the Data ───────────────────────────────────────
    // Take only the first 5 posts and project each one into a simpler
    // ProcessedPost model. The Summary is the first 80 characters of Body.

    const int maxSummaryLength = 80;

    List<ProcessedPost> processedPosts = allPosts
        .Take(5)
        .Select(post => new ProcessedPost
        {
            Id = post.Id,
            Title = post.Title,
            Summary = post.Body.Length > maxSummaryLength
                ? $"{post.Body[..maxSummaryLength].ReplaceLineEndings(" ")}..."
                : post.Body.ReplaceLineEndings(" ")
        })
        .ToList();

    logger.LogInformation("Processed {Count} posts for output", processedPosts.Count);

    // ─── Step 6: Save Results to File ───────────────────────────────────
    var fileService = new FileService(loggerFactory.CreateLogger<FileService>());
    await fileService.SaveResultsAsync(outputPath, processedPosts);

    logger.LogInformation("========================================");
    logger.LogInformation("  RealDataServiceDemo — Completed OK");
    logger.LogInformation("========================================");
}
catch (HttpRequestException ex)
{
    // Network errors, DNS failures, non-success HTTP status codes
    logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
}
catch (Exception ex)
{
    // Catch-all for unexpected errors (file I/O, deserialization, etc.)
    logger.LogCritical(ex, "Unexpected error: {Message}", ex.Message);
}
