using MysticMind.PostgresEmbed;
using Npgsql;

namespace CustomerService.Tests;

public class PostgresDatabaseFixture : IDisposable
{
    PgServer? _server;

    public string ConnectionString { get; }
    
    public PostgresDatabaseFixture()
    {
        //version of nuget (also is the postgres version)
        _server = new PgServer("10.7.1", clearInstanceDirOnStop: true, instanceId: Guid.NewGuid());
        _server.Start();
        ConnectionString = $"Server=localhost;Port={_server?.PgPort};User Id=postgres;Password=test;Database=postgres";
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _server = null;
    }

    /// <summary>
    /// Seeds the database for testing
    /// </summary>
    /// <param name="sqlFile">Path to the sql script. ex: "../../../../../database/customers.sql"</param>
    public void Seed(string sqlFile)
    {
        var sql = File.ReadAllText(sqlFile);
        using var cn = new NpgsqlConnection(ConnectionString);
        using var cmd = new NpgsqlCommand(sql, cn);
        cn.Open();
        cmd.ExecuteNonQuery();
    }
}