using System.Data;
using Dapper;
using Swytch_Api_Template.DTOs;
using Swytch_Api_Template.Models;
using Swytch.App;
using Swytch.Structures;

namespace Swytch_Api_Template.Helpers;

public static class DatabaseHelper
{
    public static void InsertSampleDataIfTablesEmpty(ISwytchApp app)
    {
        // Sample data
        var playlists = new List<AddPlaylist>
        {
            new AddPlaylist { Name = "Hip-Hop Vibes", Description = "A playlist for hip-hop lovers." },
            new AddPlaylist { Name = "Afrobeats Essentials", Description = "Groove to the best of Afrobeats." },
            new AddPlaylist
                { Name = "Alternative Sounds", Description = "A mix of soul, alternative, and mellow vibes." }
        };

        var songs = new List<Song>
        {
            // Kanye West songs
            new Song { Title = "Stronger", Artist = "Kanye West", PlaylistId = 1 },
            new Song { Title = "Gold Digger", Artist = "Kanye West", PlaylistId = 1 },
            new Song { Title = "Power", Artist = "Kanye West", PlaylistId = 1 },

            // Brymo songs
            new Song { Title = "Ara", Artist = "Brymo", PlaylistId = 3 },
            new Song { Title = "Goodbye", Artist = "Brymo", PlaylistId = 3 },
            new Song { Title = "Something Good is Happening", Artist = "Brymo", PlaylistId = 3 },

            // Asake songs
            new Song { Title = "Sungba", Artist = "Asake", PlaylistId = 2 },
            new Song { Title = "Peace Be Unto You (PBUY)", Artist = "Asake", PlaylistId = 2 },
            new Song { Title = "Joha", Artist = "Asake", PlaylistId = 2 },

            // Kweku Smoke songs
            new Song { Title = "On The Streets", Artist = "Kweku Smoke", PlaylistId = 1 },
            new Song { Title = "King Dave", Artist = "Kweku Smoke", PlaylistId = 1 }
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
        Console.WriteLine("Playlist and Songs table created");
    }
}