
namespace Swytch_Api_Template.Models;

public class Playlist
{
    public int Id { get; set; } // Auto-incremented ID
    public string Name { get; set; } // Playlist name
    public string Description { get; set; } // Optional playlist description
    public DateTime CreatedDate { get; set; } // Timestamp of when the playlist was created 
}