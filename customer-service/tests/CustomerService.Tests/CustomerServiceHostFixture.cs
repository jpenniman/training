using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CustomerService.Tests;

public class CustomerServiceHostFixture : ServiceHostFixture
{
    public CustomerServiceHostFixture()
    {
        Seed("../../../../../database/customers.sql");
    }
    
    protected override void Configuration(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder => 
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:CustomerLookup", base.ConnectionString }
            }));
    }
}