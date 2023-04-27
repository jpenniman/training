using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CustomerService.Tests;

public class ServiceHostFixture : IDisposable
{
    
    public ServiceHostFixture()
    {
       // var app = new WebApplicationFactory<Program>();
            //.WithWebHostBuilder(Configuration);
    }

    void Configuration(IWebHostBuilder builder)
    {
    }

    public void Dispose()
    {
        
    }
}