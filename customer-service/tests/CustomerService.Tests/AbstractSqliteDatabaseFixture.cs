using Microsoft.Data.Sqlite;

namespace CustomerService.Tests;

public abstract class AbstractSqliteDatabaseFixture : IDisposable
{
    public string ConnectionString => $"Filename={GetFileName()};Pooling=false";

    public AbstractSqliteDatabaseFixture()
    {
        Seed(ConnectionString);
    }

    public void Dispose()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        File.Delete(GetFileName());
    }

    protected abstract string GetFileName();
    protected abstract void Seed(string connectionString);
}