using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Swytch_Api_Template.DTOs;
using Swytch_Api_Template.Services.Interfaces;
using Swytch.Extensions;
using Swytch.Structures;

namespace Swytch_Api_Template.Actions;

public class PlaylistAction
{
    private readonly IServiceProvider _serviceProvider;

    public PlaylistAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    //Returns all playlists
    public async Task AllPlaylists(RequestContext context)
    {
        //get all playlist service
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();

        var playlists = await playlistService.GetAllPlaylists();
        await context.ToOk(playlists);
    }


    //Creates a new playlist
    public async Task CreatePlaylist(RequestContext context)
    {
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
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        var newSongJson = context.ReadJsonBody();
        var newSong = JsonSerializer.Deserialize<AddSong>(newSongJson);
        await playlistService.AddSongToPlaylist(newSong, int.Parse(playListId));
        await context.ToOk("Song added");
    }


    //Gets a particular playlist
    public async Task GetPlaylist(RequestContext context)
    {
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
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        string playListId;
        _ = context.PathParams.TryGetValue("playlistId", out playListId);
        await playlistService.DeletePlaylist(int.Parse(playListId));
        await context.ToOk($"Playlist {playListId} deleted");
    }
}