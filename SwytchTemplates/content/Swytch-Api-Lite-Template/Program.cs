// This template file is auto-generated.
// GitHub Repository:https://github.com/Gwali-1/Swytch.git 

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch_Api_Lite_Template.DTOs;
using Swytch_Api_Lite_Template.Helpers;
using Swytch_Api_Lite_Template.Services.Implementations;
using Swytch_Api_Lite_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;


ISwytchApp swytchApp = new SwytchApp(new SwytchConfig
{
    EnableStaticFileServer = true,
    StaticCacheMaxAge = "60"
});

//Enable request logging
swytchApp.AddLogging();

//Add datastore
swytchApp.AddDatastore("Data Source=playlist.db; foreign keys=true", DatabaseProviders.SQLite);

//Set up service container
ServiceCollection serviceContainer = new ServiceCollection();
//Register services here
serviceContainer.AddSingleton<ISwytchApp>(swytchApp);
serviceContainer.AddScoped<IPlaylistService, PlaylistService>();
serviceContainer.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});


//Build service provider
IServiceProvider serviceProvider = serviceContainer.BuildServiceProvider();

//Retrieving registered service
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();


//Routes and action registration
//Get all playlists 
swytchApp.AddAction("GET", "/playlists", async (context) =>
{
    logger.LogInformation("Getting all playlist");
    using var scope = serviceProvider.CreateScope();
    var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
    var playlists = await playlistService.GetAllPlaylists();
    await context.ToOk(playlists);
});


//Get a playlist
swytchApp.AddAction("GET", "/playlist/{playlistId}", async (context) =>
{
    logger.LogInformation("Getting a playlist");
    using var scope = serviceProvider.CreateScope();
    var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
    string playListId;
   var found = context.PathParams.TryGetValue("playlistId", out playListId);
   if (!found)
   {
       await context.ToBadRequest("playlistId is missing");
       return;
   }
    var playList = await playlistService.GetPlaylist(int.Parse(playListId));
    await context.ToOk(playList);
});


//create a playlist
swytchApp.AddAction("POST", "/playlist", async (context) =>
{
    logger.LogInformation("Creating new playlist");
    using var scope = serviceProvider.CreateScope();
    var playlistService = serviceProvider.GetRequiredService<IPlaylistService>();
    var newPlayList = context.ReadJsonBody<AddPlaylist>();
    await playlistService.CreatePlaylist(newPlayList);
    await context.ToOk("Playlist added");
});

//Add a song to a playlist
swytchApp.AddAction("POST", "/song/{playlistId}", async (context) =>
{
    logger.LogInformation("Adding a new song");
    using var scope = serviceProvider.CreateScope();
    var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
    string playListId;
    var found = context.PathParams.TryGetValue("playlistId", out playListId);
    if (!found)
    {
        await context.ToBadRequest("playlistId is missing");
        return;
    }
    var newSong = context.ReadJsonBody<AddSong>();
    await playlistService.AddSongToPlaylist(newSong, int.Parse(playListId));
    await context.ToOk("Song added");
});


//get songs of a playlist
swytchApp.AddAction("GET", "/songs/{playlistId}", async (context) =>
{
    logger.LogInformation("Getting  playlist songs");

    using var scope = serviceProvider.CreateScope();
    var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
    string playListId;
    var found = context.PathParams.TryGetValue("playlistId", out playListId);
    if (!found)
    {
        await context.ToBadRequest("playlistId is missing");
        return;
    }
    var songs = await playlistService.GetSongs(int.Parse(playListId));
    await context.ToOk(songs);
});

//delete a playlist
swytchApp.AddAction("DELETE", "/playlist/delete/{playlistId}", async (context) =>
{
    logger.LogInformation("Deleting  a playlist");

    using var scope = serviceProvider.CreateScope();
    var playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>();
    string playListId;
    var found = context.PathParams.TryGetValue("playlistId", out playListId);
    if (!found)
    {
        await context.ToBadRequest("playlistId is missing");
        return;
    }
    await playlistService.DeletePlaylist(int.Parse(playListId));
    await context.ToOk($"Playlist {playListId} deleted");
});

//Add Swytch API explorer page
swytchApp.AddAction("GET", "/", async (context) => { await context.ServeFile("index.html", HttpStatusCode.OK); });


//Import sample data
DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);

//Start app
await swytchApp.Listen();