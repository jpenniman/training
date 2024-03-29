using Northwind.Foundation.Api;

namespace Northwind.CustomerService.Api;

/// <summary>
/// Customer maintenance
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Sets up a new customer in the system.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The newly created customer.</returns>
    Task<Customer> OnboardCustomerAsync(OnboardCustomerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the address for a customer in the system.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The changed address.</returns>
    Task<Response<Address>> ChangeAddressAsync(ChangeAddressRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the contact information for a customer in the system.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated contact information.</returns>
    Task<Response<ContactInfo>> UpdateContactInfo(UpdateContactInfoRequest request, CancellationToken cancellationToken = default);
}