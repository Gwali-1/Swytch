using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch_Web_Template.DTOs;
using Swytch_Web_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;

namespace Swytch_Web_Template.Actions;

public class PlaylistAction
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PlaylistAction> _logger;
    private ISwytchApp _swytchApp;

    

    public PlaylistAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<PlaylistAction>>();
        _swytchApp = serviceProvider.GetRequiredService<ISwytchApp>();

    }
    
        
    public async Task Home(RequestContext context)
    {
        _logger.LogInformation("Loading landing page");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();

        var playlists = await playlistService.GetAllPlaylists();
        _swytchApp.RenderTemplate(context,"BrowsePlaylist",playlists);
        
    }



    //Returns all playlists
    public async Task AllPlaylists(RequestContext context)
    {
        _logger.LogInformation("Getting all playlist");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();

        var playlists = await playlistService.GetAllPlaylists();
        await context.ToOk(playlists);
    }


    //Creates a new playlist
    public async Task CreatePlaylist(RequestContext context)
    {
        _logger.LogInformation("Creating new playlist");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = _serviceProvider.GetRequiredService<IPlaylistService>();
        var newPlayListJson = context.ReadJsonBody();
        var newPlayList = JsonSerializer.Deserialize<AddPlaylist>(newPlayListJson);
        await playlistService.CreatePlaylist(newPlayList);
        await context.ToOk("Playlist added");
    }


    //Add a song to a playlist
    public async Task AddSong(RequestContext context)
    {
        _logger.LogInformation("Adding a new song");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        var newSongJson = context.ReadJsonBody();
        var newSong = JsonSerializer.Deserialize<AddSong>(newSongJson);
        await playlistService.AddSongToPlaylist(newSong, int.Parse(playListId));
        await context.ToOk("Song added");
    }

    public async Task GetPlaylistSongs(RequestContext context)
    {
        _logger.LogInformation("Getting playlist songs");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        var songs = await playlistService.GetSongs(int.Parse(playListId));
        await context.ToOk(songs);
    }


    //Gets a particular playlist
    public async Task GetPlaylist(RequestContext context)
    {
        _logger.LogInformation("Getting a playlist");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        var playList = await playlistService.GetPlaylist(int.Parse(playListId));
        await context.ToOk(playList);
    }

    //Deletes a playlist
    public async Task DeletePlaylist(RequestContext context)
    {
        _logger.LogInformation("Deleting  a playlist");
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        await playlistService.DeletePlaylist(int.Parse(playListId));
        await context.ToOk($"Playlist {playListId} deleted");
    }
}