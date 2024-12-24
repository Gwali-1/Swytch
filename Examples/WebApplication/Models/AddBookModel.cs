using System.Text.Json.Serialization;

namespace WebApplication.Models;

public class AddBookModel
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;
    [JsonPropertyName("genre")]
    public string Genre { get; set; } = string.Empty;
    [JsonPropertyName("publicationYear")]
    public int PublicationYear { get; set; }

    [JsonPropertyName("rating")] public int Rating { get; set; } = 1;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}