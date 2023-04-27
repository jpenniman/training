using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api;
using Northwind.CustomerService.Domain;

namespace CustomerService.Tests.Domain;

public class AddNewCustomerTests : IClassFixture<PostgresDatabaseFixture>
{
  
    [Fact]
    public async Task OnboardCustomer()
    {
        var container = new ServiceCollection()
            .AddCustomerService()
            .BuildServiceProvider();

        using var scope = container.CreateAsyncScope();
        ICustomerService customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();

        OnboardCustomerRequest onBoardCustomerRequest = new OnboardCustomerRequest("Acme Market");
        Customer savedCustomer = await customerService.OnboardCustomerAsync(onBoardCustomerRequest);

        savedCustomer.Should().NotBeNull();
        savedCustomer.CustomerNumber.Should().NotBeNullOrWhiteSpace();
        savedCustomer.Version.Should().Be(1);
    }
}