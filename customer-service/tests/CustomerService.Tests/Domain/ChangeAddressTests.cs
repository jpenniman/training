using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api.Lookup;
using Api = Northwind.CustomerService.Api;
using Northwind.CustomerService.Domain;
using Northwind.CustomerService.Lookup;

namespace CustomerService.Tests.Domain;

[Collection(nameof(DatabaseCollection))]
public class ChangeAddressTests : IDisposable
{
    readonly CustomerPostgresDatabaseFixture _databaseFixture;

    AsyncServiceScope _scope;
    IServiceProvider _scopedContainer;
    IServiceProvider _rootContainer;
    Api.ICustomerService _customerService;
    ICustomerLookup _customerLookup;
    
    public ChangeAddressTests(CustomerPostgresDatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        
        _rootContainer = new ServiceCollection()
            .AddCustomerLookup(builder => builder.UseNpgsql(_databaseFixture.ConnectionString))
            .AddCustomerService(builder => builder.UseNpgsql(_databaseFixture.ConnectionString))
            .BuildServiceProvider();

        _scope = _rootContainer.CreateAsyncScope();
        _scopedContainer = _scope.ServiceProvider;
        _customerService = _scopedContainer.GetRequiredService<Api.ICustomerService>();
        _customerLookup = _scopedContainer.GetRequiredService<ICustomerLookup>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
    
    [Fact]
    public async Task ChangeAddress()
    {
        var request = new Api.ChangeAddressRequest("ALFKI",
            new Api.Address("US")
            {
                City = "ToonTown",
                Street = "123 Main Street",
                PostalCode = "12345",
                StateOrProvince = "MA"
            }, 1);

        var response = await _customerService.ChangeAddressAsync(request, CancellationToken.None);
        response.IsSuccessful.Should().BeTrue();

        var lookupResponse = await _customerLookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest("ALFKI"));
        response.Data.Should().Be(lookupResponse.Data?.Address);

        lookupResponse.Data?.Version.Should().Be(2);
    }
}