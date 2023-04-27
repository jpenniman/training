namespace Northwind.CustomerService.Api;

public interface ICustomerService
{
    Task<Customer> OnboardCustomerAsync(OnboardCustomerRequest request, CancellationToken cancellationToken = default);
}