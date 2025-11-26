using Npgsql;

namespace MarkItDoneApi.Infra.Data;

public class DatabaseStatusChecker
{
    private readonly ConnectionFactory _connectionFactory;

    public DatabaseStatusChecker(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DatabaseStatusResult> GetDatabaseStatusAsync(string databaseName)
    {
        await using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        // database version
        await using var versionDb = new NpgsqlCommand("SHOW server_version;", connection);
        var version = await versionDb.ExecuteScalarAsync();
        
        // max connections
        await using var maxConnectionsDb = new NpgsqlCommand("SHOW max_connections;", connection);
        var maxConnections = await maxConnectionsDb.ExecuteScalarAsync();
        
        // opened connections
        await using var openedConnectionsDb = new NpgsqlCommand(
            "SELECT COUNT(*) FROM pg_stat_activity WHERE datname = @databaseName;", connection);
        openedConnectionsDb
            .Parameters
            .AddWithValue("databaseName", databaseName);
        var openedConnections = await openedConnectionsDb.ExecuteScalarAsync();

        return new DatabaseStatusResult
        {
            Version = version?.ToString() ?? "unknown",
            MaxConnections = int.TryParse(maxConnections?.ToString(), out var maxConn) ? maxConn : -1,
            OpenedConnections = int.TryParse(openedConnections?.ToString(), out var openedConn) ? openedConn : -1
        };


    }
}

public class DatabaseStatusResult
{
    public string Version { get; set; } = string.Empty;
    public int MaxConnections { get; set; }
    public int OpenedConnections { get; set; }
}
