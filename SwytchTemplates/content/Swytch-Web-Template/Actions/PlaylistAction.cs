using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorLight.Internal.Buffering;
using Swytch_Web_Template.DTOs;
using Swytch_Web_Template.Models;
using Swytch_Web_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;

namespace Swytch_Web_Template.Actions;

public class PlaylistAction
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PlaylistAction> _logger;
    private readonly ISwytchApp _swytchApp;


    public PlaylistAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<PlaylistAction>>();
        _swytchApp = serviceProvider.GetRequiredService<ISwytchApp>();
    }


    //landing page
    public async Task Home(RequestContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
        var playlists = await playlistService.GetAllPlaylists();
        await _swytchApp.RenderTemplate(context, "BrowsePlaylist", playlists);
    }

    //Creates a new playlist
    public async Task CreatePlaylist(RequestContext context)
    {
          

        if (context.Request.HttpMethod == "POST")
        {
            _logger.LogInformation("Creating new playlist");
            using var scope = _serviceProvider.CreateScope();
            var playlistService = _serviceProvider.GetRequiredService<IPlaylistService>();
            var newPlaylistFormValues = context.ReadFormBody();
            if (string.IsNullOrEmpty(newPlaylistFormValues["name"]) ||
                string.IsNullOrEmpty(newPlaylistFormValues["description"]))
            {
                await context.ToRedirect("/create-playlist");
                return;
            }

            var newPlayList = new AddPlaylist
            {
                Name = newPlaylistFormValues["name"],
                Description = newPlaylistFormValues["description"],
            };
            await playlistService.CreatePlaylist(newPlayList);
            await context.ToRedirect("/");
            return;
        }
        
        await _swytchApp.RenderTemplate<object>(context, "CreatePlaylist", null);
    }


    //Add a song to a playlist
    public async Task AddSong(RequestContext context)
    {
        if (context.Request.HttpMethod == "POST")
        {
            _logger.LogInformation("Adding a new song");
            using var scope = _serviceProvider.CreateScope();
            var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();

            var newSongFormValues = context.ReadFormBody();
            if (string.IsNullOrEmpty(newSongFormValues["playlistId"]) ||
                string.IsNullOrEmpty(newSongFormValues["artist"]) ||
                string.IsNullOrEmpty(newSongFormValues["title"]))
            {
                _logger.LogDebug("Submitted form contains empty or null fields");
                await context.ToRedirect("/create-playlist");
                return;
            }

            var newSong = new AddSong
            {
                Title = newSongFormValues["title"],
                Artist = newSongFormValues["artist"]
            };
            string playListId = newSongFormValues["playlistId"];
            await playlistService.AddSongToPlaylist(newSong, int.Parse(playListId));

            await context.ToRedirect($"/playlist/{playListId}");
            return;
        }


        await _swytchApp.RenderTemplate<object>(context, "AddSong", null);
    }


    //Deletes a playlist
    public async Task DeletePlaylist(RequestContext context)
    {
        if (context.Request.HttpMethod == "POST")
        {
            _logger.LogInformation("Deleting  a playlist");
            using var scope = _serviceProvider.CreateScope();
            var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
            var deletePlaylistFormValues = context.ReadFormBody();
            if (string.IsNullOrEmpty(deletePlaylistFormValues["playlistId"]))
            {
                _logger.LogDebug("Submitted form contains empty or null fields");
                await context.ToRedirect("/delete-playlist");
            }

            string playListId = deletePlaylistFormValues["playlistId"];
            await playlistService.DeletePlaylist(int.Parse(playListId));
            await context.ToRedirect("/");
            return;
        }

        await _swytchApp.RenderTemplate<object>(context, "DeletePlaylist", null);
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
        var songs = await playlistService.GetSongs(int.Parse(playListId));
        var viewList = new ViewList
        {
            Playlist = playList,
            Songs = songs
        };

        await _swytchApp.RenderTemplate<ViewList>(context, "ViewPlaylist", viewList);
    }
}