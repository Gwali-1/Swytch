// This template file is auto-generated.
// GitHub Repository:https://github.com/Gwali-1/Swytch.git 

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch_Api_Template.Actions;
using Swytch_Api_Template.Helpers;
using Swytch_Api_Template.Services.Implementations;
using Swytch_Api_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;


ISwytchApp swytchApp = new SwytchApp(new SwytchConfig
{
    EnableStaticFileServer = true,
    StaticCacheMaxAge = "60"
});

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

//Actions instance
PlaylistAction playlistAction = new PlaylistAction(serviceProvider);


//Register routes with action
swytchApp.AddAction("GET", "/playlists", playlistAction.AllPlaylists);
swytchApp.AddAction("GET", "/playlist/{playlistId}", playlistAction.GetPlaylist);
swytchApp.AddAction("POST", "/playlist", playlistAction.CreatePlaylist);
swytchApp.AddAction("POST", "/song/{playlistId}", playlistAction.AddSong);
swytchApp.AddAction("GET", "/songs/{playlistId}", playlistAction.GetPlaylistSongs);
swytchApp.AddAction("DELETE", "/playlist/delete/{playlistId}", playlistAction.DeletePlaylist);


//Add api explorer page
swytchApp.AddAction("GET", "/", async (context) => { await context.ServeFile("index.html", HttpStatusCode.OK); });
    
//Import sample data
DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);

//start app
await swytchApp.Listen();