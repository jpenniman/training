using MysticMind.PostgresEmbed;
using Npgsql;

namespace CustomerService.Tests;

public abstract class PostgresDatabaseFixture : IDisposable
{
    PgServer? _server;

    static volatile int _port = 5500;
    
    public string ConnectionString { get; }
    
    // No it can't xUnit requires it be public. Don't listed to R#.
    // ReSharper disable once MemberCanBeProtected.Global
    // ReSharper disable once PublicConstructorInAbstractClass
    public PostgresDatabaseFixture()
    {
        _server = new PgServer(
            "10.7.1", // version of nuget (also is the postgres version). 10.7.1 is the last supported version.
            clearInstanceDirOnStop: true, // deletes the instance/databases on stop  
            instanceId: Guid.NewGuid(), // defines this as a unique instance so each test class gets its own database and don't conflict with each other.
            port: Interlocked.Increment(ref _port)); // each instance needs its own port.
        _server.Start();
        ConnectionString = $"Server=localhost;Port={_server?.PgPort};User Id=postgres;Password=test;Database=postgres";
    }

    public virtual void Dispose()
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