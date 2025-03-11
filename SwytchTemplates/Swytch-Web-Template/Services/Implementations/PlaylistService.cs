using Dapper;
using Swytch_Web_Template.DTOs;
using Swytch_Web_Template.Models;
using Swytch_Web_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Structures;

namespace Swytch_Web_Template.Services.Implementations;

public class PlaylistService : IPlaylistService
{
    private readonly ISwytchApp _app;

    public PlaylistService(ISwytchApp swytchApp)
    {
        _app = swytchApp;
    }


    public Task CreatePlaylist(AddPlaylist newPlaylist)
    {
        string query = "INSERT INTO Playlist (Name, Description) VALUES (@Name, @Description)";
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        dbContext.Execute(query, newPlaylist);
        return Task.CompletedTask;
    }

    public Task<Playlist?> GetPlaylist(int playlistId)
    {
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        string query = "SELECT Id, Name, Description, CreatedDate FROM Playlist WHERE Id = @PlaylistId";

        var playList = dbContext.Query<Playlist>(query, new { PlaylistId = playlistId });
        if (!playList.Any())
        {
            return Task.FromResult(default(Playlist));
        }

        return Task.FromResult(playList.First());
    }

    public Task DeletePlaylist(int playlistId)
    {
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        string deletePlaylistQuery = "DELETE FROM Playlist WHERE Id = @PlaylistId";
        dbContext.Execute(deletePlaylistQuery, new { PlaylistId = playlistId });
        return Task.CompletedTask;
    }

    public Task<List<Playlist>> GetAllPlaylists()
    {
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        string query = "SELECT Id, Name, Description, CreatedDate FROM Playlist";

        var playlists = dbContext.Query<Playlist>(query).ToList();
        return Task.FromResult(playlists);
    }

    public Task AddSongToPlaylist(AddSong newSong, int playlistId)
    {
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        string query = "INSERT INTO Song (Title, Artist, PlaylistId) VALUES (@Title, @Artist, @PlaylistId)";

        var song = new
        {
            Title = newSong.Title,
            Artist = newSong.Artist,
            PlaylistId = playlistId
        };

        dbContext.Execute(query, song);
        return Task.CompletedTask;
    }

    public Task<List<Song>> GetSongs(int playListId)
    {
        using var dbContext = _app.GetConnection(DatabaseProviders.SQLite);
        string query = "SELECT Id ,Title, Artist FROM Song  WHERE PlaylistId = @PlaylistId";
        var songs = dbContext.Query<Song>(query, new { PlaylistId = playListId }).ToList();
        return Task.FromResult(songs);
    }
}