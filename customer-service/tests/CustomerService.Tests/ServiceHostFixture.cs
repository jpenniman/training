using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CustomerService.Tests;

// The database needs to be created first. Inheriting from the database fixture ensures we have a database server
// available and can get its connection string.
public abstract class ServiceHostFixture : PostgresDatabaseFixture
{
    WebApplicationFactory<Program>? _app;
    
    protected ServiceHostFixture()
    {
        _app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(Configuration);

        HttpMessageHandler = _app.Server.CreateHandler();
    }

    public HttpMessageHandler HttpMessageHandler { get; }

    protected abstract void Configuration(IWebHostBuilder builder);

    public override void Dispose()
    {
        _app?.Dispose(); // tear down the service host
        _app = null;
        base.Dispose(); // tear down the database
    }
}