using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api.Lookup;
using Northwind.CustomerService.Client;
using Xunit.Abstractions;

namespace CustomerService.Tests;

public class IntegrationTests : IClassFixture<CustomerServiceHostFixture>
{
    readonly ITestOutputHelper _output;
    readonly CustomerServiceHostFixture _serviceHost;

    public IntegrationTests(CustomerServiceHostFixture serviceHost, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _output = output;
    }

    [Fact]
    public async Task FindByCustomerNo()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddCustomerLookupClient(configuration, _serviceHost.HttpMessageHandler);
        var container = services.BuildServiceProvider(true);

        var lookup = container.GetRequiredService<ICustomerLookup>();
        var response = await lookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest("ALFKI"));
        
        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data?.CustomerNumber.Should().Be("ALFKI");
        response.Data?.CompanyName.Should().Be("Alfreds Futterkiste");
    }
    
    [Fact]
    public async Task FindByCompanyName()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddCustomerLookupClient(configuration, _serviceHost.HttpMessageHandler);
        var container = services.BuildServiceProvider(true);

        var lookup = container.GetRequiredService<ICustomerLookup>();
        var response = await lookup.FindByCompanyNameAsync(new FindByCompanyNameRequest("An"));

        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.TotalCount.Should().Be(2);
    }
}