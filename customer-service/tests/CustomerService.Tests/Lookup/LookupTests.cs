using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api;
using Northwind.CustomerService.Api.Lookup;
using Northwind.CustomerService.Lookup;
using Northwind.Foundation.Api;

namespace CustomerService.Tests.Lookup;

public class LookupTests : IClassFixture<PostgresDatabaseFixture>
{
    readonly PostgresDatabaseFixture _postgresDatabaseFixture;

    public LookupTests(PostgresDatabaseFixture postgresDatabaseFixture)
    {
        _postgresDatabaseFixture = postgresDatabaseFixture;
        postgresDatabaseFixture.Seed("../../../../../database/customers.sql");
    }

    [Fact]
    public async Task FindByCustomerNo()
    {
        var container = new ServiceCollection()
            .AddCustomerLookup(options => options.UseNpgsql(_postgresDatabaseFixture.ConnectionString))
            .BuildServiceProvider();
    
        ICustomerLookup customerLookup = container.GetRequiredService<ICustomerLookup>();
        
        Response<Customer> response = await customerLookup.FindByCustomerNoAsync(new FindByCustomerNoRequest("ALFKI"));

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
        
        PagedResponse<Customer> response = await customerLookup.FindByCompanyNameAsync(new FindByCompanyNameRequest("An"));

        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.TotalCount.Should().Be(2);
    }
}