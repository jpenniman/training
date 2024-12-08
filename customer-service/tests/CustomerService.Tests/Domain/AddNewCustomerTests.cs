using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api.Lookup;
using Api = Northwind.CustomerService.Api;
using Northwind.CustomerService.Domain;
using Northwind.CustomerService.Lookup;

namespace CustomerService.Tests.Domain;

[Collection(nameof(DatabaseCollection))]
public class AddNewCustomerTests
{
    readonly CustomerPostgresDatabaseFixture _databaseFixture;

    public AddNewCustomerTests(CustomerPostgresDatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task OnboardCustomer()
    {
        var container = new ServiceCollection()
            .AddCustomerLookup(builder => builder.UseNpgsql(_databaseFixture.ConnectionString))
            .AddCustomerService(builder => builder.UseNpgsql(_databaseFixture.ConnectionString))
            .BuildServiceProvider();

        await using var scope = container.CreateAsyncScope();
        Api.ICustomerService customerService = scope.ServiceProvider.GetRequiredService<Api.ICustomerService>();

        Api.OnboardCustomerRequest onBoardCustomerRequest = 
            new Api.OnboardCustomerRequest("Acme Market", new Api.Address("USA"), new Api.ContactInfo("555-555-5555"));
        Api.Customer savedCustomer = await customerService.OnboardCustomerAsync(onBoardCustomerRequest);

        var lookup = scope.ServiceProvider.GetRequiredService<ICustomerLookup>();
        var response = await lookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest(savedCustomer.CustomerNumber));
        var dbCustomer = response.Data;

        dbCustomer.Should().NotBeNull();
        savedCustomer.Should().NotBeNull();
        savedCustomer.CustomerNumber.Should().Be(dbCustomer?.CustomerNumber);
        savedCustomer.Version.Should().Be(1);
        dbCustomer?.Version.Should().Be(1);
    }
}