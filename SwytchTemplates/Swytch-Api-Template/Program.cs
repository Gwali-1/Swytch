// This template file is auto-generated.
// GitHub Repository:https://github.com/Gwali-1/Swytch.git 
// See <template url> for more information
using Microsoft.Extensions.DependencyInjection;
using Swytch_Api_Template.Actions;
using Swytch_Api_Template.Helpers;
using Swytch_Api_Template.Services.Implementations;
using Swytch_Api_Template.Services.Interfaces;
using Swytch.App;
using Swytch.Structures;


ISwytchApp swytchApp = new SwytchApp();

//Add datastore
swytchApp.AddDatastore("Data Source=playlist.db", DatabaseProviders.SQLite);

ServiceCollection serviceContainer = new ServiceCollection();
//Register services here
serviceContainer.AddSingleton<ISwytchApp>(swytchApp);
serviceContainer.AddScoped<IPlaylistService, PlaylistService>(c => new PlaylistService(c));


//build service provider and use
IServiceProvider serviceProvider = serviceContainer.BuildServiceProvider();

//actions
PlaylistAction playlistAction = new PlaylistAction(serviceProvider);


//register 
swytchApp.AddAction("GET", "/playlists", playlistAction.AllPlaylists);
swytchApp.AddAction("GET", "/playlist/{playlistId}", playlistAction.GetPlaylist);
swytchApp.AddAction("POST", "/playlist", playlistAction.CreatePlaylist);
swytchApp.AddAction("POST", "/Song/{playlistId}", playlistAction.AddSong);
swytchApp.AddAction("DELETE", "/playlist/{playlistId}", playlistAction.DeletePlaylist);



//migrate data

DatabaseHelper.CreateTablesIfNotExist(swytchApp);
DatabaseHelper.InsertSampleDataIfTablesEmpty(swytchApp);

//start app
await swytchApp.Listen();