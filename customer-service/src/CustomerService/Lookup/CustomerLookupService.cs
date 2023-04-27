using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Api;
using Northwind.CustomerService.Api.Lookup;
using Northwind.Foundation.Api;
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

    public async Task<Response<Customer>> FindByCustomerNoAsync(FindByCustomerNoRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _db.Set<Customer>()
                .Where(c => c.CustomerNumber == request.CustomerNo)
                .SingleOrDefaultAsync(cancellationToken);

            return new Response<Customer>(customer);
        }
        catch (Exception ex)
        {
            const string detail = "See Log for details.";

            var error = new Error(CODE, TITLE, detail);
            _logger.Error(ex, "Error: {@error}", error);
            return new Response<Customer>(new[] { error });
        }
    }

    public async Task<PagedResponse<Customer>> FindByCompanyNameAsync(FindByCompanyNameRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var comparisonValue = $"{request.PartialCompanyName}%".ToLowerInvariant();
            
            var query = _db.Set<Customer>()
                .Where(c => EF.Functions.Like(c.CompanyName.ToLower(), comparisonValue));

            var customers = await query.Skip(request.Skip).Take(request.Take).ToArrayAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);
            return new PagedResponse<Customer>(customers, totalCount);
        }
        catch (Exception ex)
        {
            const string detail = "See Log for details.";

            var error = new Error(CODE, TITLE, detail);
            _logger.Error(ex, "Error: {@error}", error);
            return new PagedResponse<Customer>(new[] { error } );
        }
    }
}