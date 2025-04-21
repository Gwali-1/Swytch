
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch_Web_Template.Actions;
using Swytch_Web_Template.Helpers;
using Swytch_Web_Template.Services.Implementations;
using Swytch_Web_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Structures;

ISwytchApp  swytchApp = new SwytchApp(new SwytchConfig
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



//Action instance
PlaylistAction playlistAction = new PlaylistAction(serviceProvider);


//Register routes with actions
swytchApp.AddAction("GET","/", playlistAction.Home);
swytchApp.AddAction("POST,GET", "/create-playlist", playlistAction.CreatePlaylist);
swytchApp.AddAction("POST,GET", "/add-song", playlistAction.AddSong);
swytchApp.AddAction("POST,GET", "/delete-playlist", playlistAction.DeletePlaylist);
swytchApp.AddAction("GET", "/playlist/{playlistId}", playlistAction.GetPlaylist);



//Import sample data
DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);


await swytchApp.Listen();

