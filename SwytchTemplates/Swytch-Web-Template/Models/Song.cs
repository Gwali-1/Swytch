namespace Swytch_Web_Template.Models;

public class Song
{
    public int Id { get; set; }            // Auto-incremented ID
    public string Title { get; set; }      // Song title
    public string Artist { get; set; }     // Song artist
    public int PlaylistId { get; set; }    // Foreign key linking to the Playlist
}