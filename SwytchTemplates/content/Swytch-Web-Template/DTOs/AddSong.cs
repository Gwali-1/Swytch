using System.Text.Json.Serialization;

namespace Swytch_Web_Template.DTOs;

public class AddSong
{
    [JsonPropertyName("title")] public string Title { get; set; } // Title of the song
    [JsonPropertyName("artist")] public string Artist { get; set; } // Artist of the song
}