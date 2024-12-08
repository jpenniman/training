using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api.Lookup;
using Northwind.CustomerService.Domain;
using Northwind.CustomerService.Lookup;
using Serilog;
using Xunit.Abstractions;
using Api = Northwind.CustomerService.Api;

namespace CustomerService.Tests.Domain;

[Collection(nameof(DatabaseCollection))]
public class UpdateContactInfoTests
{
    readonly CustomerPostgresDatabaseFixture _databaseFixture;

    AsyncServiceScope _scope;
    IServiceProvider _scopedContainer;
    IServiceProvider _rootContainer;
    Api.ICustomerService _customerService;
    ICustomerLookup _customerLookup;
    
    public UpdateContactInfoTests(CustomerPostgresDatabaseFixture databaseFixture, ITestOutputHelper testOutput)
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
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(testOutput)
            .CreateLogger();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
    
    [Fact]
    public async Task UpdateContactInfo()
    {
        var original = await _customerLookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest("AROUT"));
        var request = new Api.UpdateContactInfoRequest("AROUT",
            new Api.ContactInfo("555-555-5555")
            {
                Name = original.Data.ContactInfo.Name,
                Title = original.Data.ContactInfo.Title,
                FaxNumber = original.Data.ContactInfo.FaxNumber
            });

        request.Version = original.Data.Version;

        var response = await _customerService.UpdateContactInfo(request, CancellationToken.None);
        response.IsSuccessful.Should().BeTrue();

        var updatedCustomer = await _customerLookup.FindByCustomerNoAsync(new FindByCustomerNumberRequest("AROUT"));
        response.Data.Should().Be(updatedCustomer.Data?.ContactInfo);

        updatedCustomer.Data?.Version.Should().Be(original.Data.Version + 1);
    }
}