

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



//register 

//browse playlist
//create playlist
//add song
//delete song 
//view playliat

swytchApp.AddAction("GET","/", playlistAction.Home);
swytchApp.AddAction("POST,GET", "/create-playlist", playlistAction.CreatePlaylist);

swytchApp.AddAction("GET", "/playlist/{playlistId}", playlistAction.GetPlaylist);
swytchApp.AddAction("POST", "/song/{playlistId}", playlistAction.AddSong);
swytchApp.AddAction("GET", "/songs/{playlistId}", playlistAction.GetPlaylistSongs);
swytchApp.AddAction("DELETE", "/playlist/delete/{playlistId}", playlistAction.DeletePlaylist);


//migrate data

DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);


// swytchApp.AddAction("GET","/", async (context) =>
// {
//     await swytchApp.RenderTemplate<Object>(context,"BrowsePlaylist",null);
// });
//


await swytchApp.Listen();

