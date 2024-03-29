using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api;
using Northwind.CustomerService.Api.Lookup;
using Northwind.CustomerService.Lookup;
using Northwind.Foundation.Api;
using Serilog;
using Xunit.Abstractions;

namespace CustomerService.Tests.Lookup;

[Collection(nameof(DatabaseCollection))]
public class LookupTests
{
    readonly ITestOutputHelper _testOutput;
    readonly CustomerPostgresDatabaseFixture _postgresDatabaseFixture;

    public LookupTests(CustomerPostgresDatabaseFixture postgresDatabaseFixture, ITestOutputHelper testOutput)
    {
        _postgresDatabaseFixture = postgresDatabaseFixture;
        _testOutput = testOutput;
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(_testOutput)
            .CreateLogger();
    }

    [Fact]
    public async Task FindByCustomerNo()
    {
        var container = new ServiceCollection()
            .AddCustomerLookup(options => options.UseNpgsql(_postgresDatabaseFixture.ConnectionString))
            .BuildServiceProvider();
    
        ICustomerLookup customerLookup = container.GetRequiredService<ICustomerLookup>();
        
        Response<Customer> response = await customerLookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest("ALFKI"));
        
        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data?.CustomerNumber.Should().Be("ALFKI");
        response.Data?.CompanyName.Should().Be("Alfreds Futterkiste");
    }
    
    [Fact]
    public async Task FindByCompanyName()
    {
        var container = new ServiceCollection()
            .AddCustomerLookup(options => options.UseNpgsql(_postgresDatabaseFixture.ConnectionString))
            .BuildServiceProvider();
    
        ICustomerLookup customerLookup = container.GetRequiredService<ICustomerLookup>();
        
        PagedResponse<CustomerLookupResult> response = await customerLookup.FindByCompanyNameAsync(new FindByCompanyNameRequest("An"));

        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.TotalCount.Should().Be(2);
    }
}