using System.ServiceModel;
using Northwind.Foundation.Api;

namespace Northwind.CustomerService.Api.Lookup;

/// <summary>
/// Customer Lookup functionality.
/// </summary>
[ServiceContract]
public interface ICustomerLookup
{
    /// <summary>
    /// Retrieves the customer for the supplied customer number.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [OperationContract]
    Task<Response<Customer>> FindByCustomerNoAsync(FindByCustomerNoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of customers whose company name starts with the provided search term.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [OperationContract]
    Task<PagedResponse<Customer>> FindByCompanyNameAsync(FindByCompanyNameRequest request, CancellationToken cancellationToken = default);
}