namespace Swytch.Structures;


/// <summary>
/// Enum of some database providers to be used when adding a data storage to your swytch app.
/// </summary>
public enum DatabaseProviders
{
    /// <summary>
    /// Microsoft SQL Server
    /// </summary>
    SqlServer = 1,

    /// <summary>
    /// MySQL
    /// </summary>
    MySql = 2,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSql = 3,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite = 4,

    /// <summary>
    /// Oracle
    /// </summary>
    Oracle = 5,
}