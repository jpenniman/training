using Northwind.CustomerService.Api;

namespace Northwind.CustomerService.Domain;

sealed class CustomerService : ICustomerService
{
    public Task<Customer> OnboardCustomerAsync(OnboardCustomerRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}