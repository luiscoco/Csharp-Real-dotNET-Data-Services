// =============================================================================
// FileService.cs — Handles writing processed data to disk.
//
// This service:
//   1. Serializes the processed results back to formatted JSON.
//   2. Creates the output directory if it does not already exist.
//   3. Writes the JSON string to disk asynchronously.
//
// Using async file I/O is a best practice — it frees the thread while
// the OS completes the disk write, which matters in high-throughput apps.
// =============================================================================

using System.Text.Json;
using Microsoft.Extensions.Logging;
using RealDataServiceDemo.Models;

namespace RealDataServiceDemo.Services;

/// <summary>
/// Writes processed post data to a local JSON file.
/// </summary>
public class FileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Serializes and saves a list of <see cref="ProcessedPost"/> to a JSON file.
    /// </summary>
    /// <param name="filePath">Full path where the output file will be created.</param>
    /// <param name="processedPosts">The data to serialize and save.</param>
    public async Task SaveResultsAsync(string filePath, List<ProcessedPost> processedPosts)
    {
        _logger.LogInformation("Preparing to save {Count} processed posts to {Path}", processedPosts.Count, filePath);

        // Ensure the output directory exists
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            _logger.LogInformation("Created output directory: {Directory}", directory);
        }

        // Configure the serializer to produce human-readable, indented JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Serialize the data to a JSON string
        string jsonContent = JsonSerializer.Serialize(processedPosts, options);

        // Write the JSON string to disk asynchronously
        await File.WriteAllTextAsync(filePath, jsonContent);

        _logger.LogInformation("Successfully saved output to {Path}", filePath);
    }
}
