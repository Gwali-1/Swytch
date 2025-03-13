using System.Data;
using Dapper;
using Swytch_Api_Lite_Template.DTOs;
using Swytch_Api_Lite_Template.Models;
using Swytch.App;
using Swytch.Structures;

namespace Swytch_Api_Lite_Template.Helpers;

public static class DatabaseHelper
{
    public static void InsertSampleDataIfTablesEmpty(ISwytchApp app)
    {
        // Sample data
        var playlists = new List<AddPlaylist>
        {
            new AddPlaylist
            {
                Name = "Asakaa",
                Description =
                    "🔊 Raw energy, street vibes, and the hardest drill beats straight from Ghana. If you know, you know. 🚀🔥"
            },
            new AddPlaylist
            {
                Name = "Wake Up Mr. West",
                Description =
                    "🎨 A celebration of Ye’s genius—soul samples, stadium anthems, and pure artistry from one of the greatest. 🏆🎶"
            },
            new AddPlaylist
            {
                Name = "Author's Picks",
                Description =
                    "✨ Real melodies if you ask me. No skips, just vibes. Let this one play front to back. 🎼💫"
            },
            new AddPlaylist
            {
                Name = "The Ghana List",
                Description =
                    "🇬🇭 A journey through the sounds of Ghana—from highlife classics to the freshest drill & rap anthems. 🎵🔥"
            }
        };

        var songs = new List<Song>
        {
            // Asakaa Playlist (PlaylistId = 1)
            new Song { Title = "Mmrepa", Artist = "Kweku Smoke", PlaylistId = 1 },
            new Song { Title = "Young Boy", Artist = "Kweku Smoke", PlaylistId = 1 },
            new Song { Title = "Kwadwo", Artist = "Kweku Smoke", PlaylistId = 1 },
            new Song { Title = "Dreams", Artist = "Kweku Smoke", PlaylistId = 1 },
            new Song { Title = "Letter to You", Artist = "Ypee Okenneth", PlaylistId = 1 },
            new Song { Title = "Oh My Days", Artist = "Ypee Okenneth", PlaylistId = 1 },
            new Song { Title = "Badman", Artist = "Jahbad", PlaylistId = 1 },

            // Wake Up Mr. West Playlist (PlaylistId = 2)
            new Song { Title = "Good Morning", Artist = "Kanye West", PlaylistId = 2 },
            new Song { Title = "Touch the Sky", Artist = "Kanye West", PlaylistId = 2 },
            new Song { Title = "Can't Tell Me Nothing", Artist = "Kanye West", PlaylistId = 2 },
            new Song { Title = "Flashing Lights", Artist = "Kanye West", PlaylistId = 2 },
            new Song { Title = "Through the Wire", Artist = "Kanye West", PlaylistId = 2 },
            new Song { Title = "Champion", Artist = "Kanye West", PlaylistId = 2 },

            // Author's Picks Playlist (PlaylistId = 3)
            new Song { Title = "All the Love", Artist = "Ayra Starr", PlaylistId = 3 },
            new Song { Title = "Pray for Me", Artist = "Saint Jhn", PlaylistId = 3 },
            new Song { Title = "Deja Vu", Artist = "Seyi Vibez", PlaylistId = 3 },
            new Song { Title = "On Form", Artist = "Burna Boy", PlaylistId = 3 },
            new Song { Title = "Ransom", Artist = "Saint Jhn", PlaylistId = 3 },

            // The Ghana List Playlist (PlaylistId = 4)
            new Song { Title = "YAYA", Artist = "Black Sherif", PlaylistId = 4 },
            new Song { Title = "Rollies and Cigars", Artist = "Sarkodie", PlaylistId = 4 },
            new Song { Title = "Most Original", Artist = "Stonebwoy", PlaylistId = 4 },
            new Song { Title = "Susuka", Artist = "Kofi Kinaata", PlaylistId = 4 },
            new Song { Title = "Aben Wo Ha", Artist = "Daddy Lumba", PlaylistId = 4 }
        };


        // Insert data into the database
        using IDbConnection dbConnection = app.GetConnection(DatabaseProviders.SQLite);
        dbConnection.Open();

        // Check if the Playlist table is empty
        int playlistCount = dbConnection.ExecuteScalar<int>("SELECT COUNT(*) FROM Playlist;");
        if (playlistCount == 0)
        {
            // Insert playlists
            foreach (var playlist in playlists)
            {
                dbConnection.Execute(
                    @"INSERT INTO Playlist (Name, Description)
                      VALUES (@Name, @Description);", playlist);
            }

            // Insert songs
            foreach (var song in songs)
            {
                dbConnection.Execute(
                    @"INSERT INTO Song (Title, Artist, PlaylistId)
                      VALUES (@Title, @Artist, @PlaylistId);", song);
            }
        }
    }

    public static void CreateTablesIfNotExist(ISwytchApp app)
    {
        using IDbConnection dbConnection = app.GetConnection(DatabaseProviders.SQLite);
        dbConnection.Open();

        // SQL script to create Playlist and Song tables
        string createTablesSql = @"
                -- Create Playlist table if it doesn't exist
                CREATE TABLE IF NOT EXISTS Playlist (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,   -- INTEGER for auto-incrementing ID
                    Name TEXT NOT NULL,                     -- TEXT for playlist name
                    Description TEXT,                       -- TEXT for description (nullable)
                    CreatedDate TEXT DEFAULT (CURRENT_TIMESTAMP)  --  datetime 
                );

                -- Create Song table if it doesn't exist
                CREATE TABLE IF NOT EXISTS Song (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,   -- INTEGER for auto-incrementing ID
                    Title TEXT NOT NULL,                    -- TEXT for song title
                    Artist TEXT,                            -- TEXT for artist name (nullable)
                    PlaylistId INTEGER NOT NULL,            -- FOREIGN KEY referencing Playlist
                    FOREIGN KEY (PlaylistId) REFERENCES Playlist(Id) ON DELETE CASCADE  -- Foreign key with cascading delete
                );

            ";

        dbConnection.Execute(createTablesSql);
    }
}