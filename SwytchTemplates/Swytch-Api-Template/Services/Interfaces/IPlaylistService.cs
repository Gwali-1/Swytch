using Swytch_Api_Template.DTOs;
using Swytch_Api_Template.Models;

namespace Swytch_Api_Template.Services.Interfaces;

public interface IPlaylistService
{
    Task CreatePlaylist(AddPlaylist newPlaylist);
    Task<Playlist> GetPlaylist(int playlistId);
    Task DeletePlaylist(int playlistId);
    Task<List<Playlist>> GetAllPlaylists();
    Task AddSongToPlaylist(AddSong newSong, int playlistId);
}