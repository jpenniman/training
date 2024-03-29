using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Api.Lookup;
using Northwind.Foundation.Api;
using Customer = Northwind.CustomerService.Api.Customer;
using ILogger = Serilog.ILogger;

namespace Northwind.CustomerService.Lookup;

sealed class CustomerLookupService : ICustomerLookup
{
    const string CODE = "SEARCH_ERROR";
    const string TITLE = "An error occurred while trying to perform a search.";
    
    readonly CustomerLookupBoundedContext _db;

    static readonly ILogger _logger = Serilog.Log.ForContext<CustomerLookupService>();

    public CustomerLookupService(CustomerLookupBoundedContext db)
    {
        _db = db;
    }

    public async Task<Response<Customer>> FindByCustomerNoAsync(FindByCustomerNumberRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _db.Set<Customer>()
                .Where(c => c.CustomerNumber == request.CustomerNumber)
                .SingleOrDefaultAsync(cancellationToken);

            return new Response<Customer>(true, customer);
        }
        catch (Exception ex)
        {
            const string detail = "See Log for details.";

            var error = new Error(CODE, TITLE, detail);
            _logger.Error(ex, "Error: {@error}", error);
            return new Response<Customer>(false, errors: new[] { error });
        }
    }

    public async Task<PagedResponse<CustomerLookupResult>> FindByCompanyNameAsync(FindByCompanyNameRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var comparisonValue = $"{request.PartialCompanyName}%".ToLowerInvariant();
            
            var query = _db.Set<Customer>()
                .Where(c => EF.Functions.Like(c.CompanyName.ToLower(), comparisonValue));

            var customers = await query.Skip(request.Skip).Take(request.Take)
                .Select(c => Map(c))
                .ToArrayAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);
            return new PagedResponse<CustomerLookupResult>(customers, totalCount);
        }
        catch (Exception ex)
        {
            const string detail = "See Log for details.";

            var error = new Error(CODE, TITLE, detail);
            _logger.Error(ex, "Error: {@error}", error);
            return new PagedResponse<CustomerLookupResult>(new[] { error } );
        }
    }

    static CustomerLookupResult Map(Customer customer)
    {
        return new CustomerLookupResult(
            customer.CustomerNumber, customer.CompanyName, customer.Address?.Country, customer.ContactInfo?.PhoneNumber);
    }
}