

using System.Collections.Immutable;
using System.Threading.Channels;
using Azure.Core;
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
    StaticCacheMaxAge = "5"
});
swytchApp.AddLogging();
//Add datastore
swytchApp.AddDatastore("Data Source=playlist.db; foreign keys=true", DatabaseProviders.SQLite);



ServiceCollection serviceContainer = new ServiceCollection();

//Register services here
serviceContainer.AddSingleton<ISwytchApp>(swytchApp);
serviceContainer.AddScoped<IPlaylistService, PlaylistService>();
serviceContainer.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
//build service provider and use
IServiceProvider serviceProvider = serviceContainer.BuildServiceProvider();



//actions
PlaylistAction playlistAction = new PlaylistAction(serviceProvider);


swytchApp.AddAction("GET","/", playlistAction.Home);
swytchApp.AddAction("POST,GET", "/create-playlist", playlistAction.CreatePlaylist);
swytchApp.AddAction("POST,GET", "/add-song", playlistAction.AddSong);
swytchApp.AddAction("POST,GET", "/delete-playlist", playlistAction.DeletePlaylist);
swytchApp.AddAction("GET", "/playlist/{playlistId}", playlistAction.GetPlaylist);




//migrate data
DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);


await swytchApp.Listen();

