using System.Text.Json.Serialization;

namespace Swytch_Api_Lite_Template.DTOs;

public class AddPlaylist

{
    [JsonPropertyName("name")]
    public string Name { get; set; }            // Name of the playlist
    [JsonPropertyName("description")]
    public string Description { get; set; }    // Description of the playlist (optional)
}