using Mapster;
using Northwind.CustomerService.Api;
using Northwind.Foundation.Api;

namespace Northwind.CustomerService.Domain;

sealed class CustomerService : ICustomerService
{
    readonly BoundedContext _boundedContext;

    public CustomerService(BoundedContext boundedContext)
    {
        TypeAdapterConfig<Api.Address, Address>.NewConfig().MapToConstructor(true);
        TypeAdapterConfig<Address, Api.Address>.NewConfig().MapToConstructor(true);
        
        TypeAdapterConfig<Api.ContactInfo, ContactInfo>.NewConfig().MapToConstructor(true);
        TypeAdapterConfig<ContactInfo, Api.ContactInfo>.NewConfig().MapToConstructor(true);
        
        _boundedContext = boundedContext;
    }

    public async Task<Api.Customer> OnboardCustomerAsync(OnboardCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = Customer.Create(
            request.CompanyName,
            request.Address.Adapt<Address>(),
            request.ContactInfo.Adapt<ContactInfo>());
        
        _boundedContext.Customers.Add(customer);
        await _boundedContext.SaveChangesAsync(cancellationToken);

        return customer.Adapt<Api.Customer>();
    }

    public async Task<Response<Api.Address>> ChangeAddressAsync(ChangeAddressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _boundedContext.Customers.FindAsync(CustomerNumber.Parse(request.CustomerNumber));
            if (customer is null)
            {
                return new Response<Api.Address>(false, errors: new[]
                {
                    new Error(
                        "NOT_FOUND",
                        "Customer not found.",
                        $"Customer number {request.CustomerNumber} not found.")
                });
            }

            if (customer.Version != request.Version)
            {
                var response = new Response<Api.Address>(false,
                    data: customer.Address.Adapt<Api.Address>(),
                    errors: new[]
                    {
                        new Error(
                            "VERSION_CONFLICT",
                            "The customer has been modified since last viewed.",
                            $"Customer number {request.CustomerNumber} was last viewed with version {request.Version}, but is currently at version {customer.Version}.")
                    });
                return response;
            }

            // TODO: Resolve address with online service

            customer.ChangeAddress(request.NewAddress.Adapt<Address>());
            //customer.Version += 1;

            _boundedContext.Customers.Update(customer);
            await _boundedContext.SaveChangesAsync(cancellationToken);

            return new Response<Api.Address>(true, customer.Address.Adapt<Api.Address>());
        }
        catch (Exception ex)
        {
            return new Response<Api.Address>(false, errors: new[]
            {
                new Error(
                    "ERROR",
                    ex.Message,
                    ex.ToString())
            });
        }
    }

    public async Task<Response<Api.ContactInfo>> UpdateContactInfo(UpdateContactInfoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _boundedContext.Customers.FindAsync(CustomerNumber.Parse(request.CustomerNumber));
            if (customer is null)
            {
                return new Response<Api.ContactInfo>(false, errors: new[]
                {
                    new Error(
                        "NOT_FOUND",
                        "Customer not found.",
                        $"Customer number {request.CustomerNumber} not found.")
                });
            }

            if (customer.Version != request.Version)
            {
                var response = new Response<Api.ContactInfo>(false,
                    data: customer.ContactInfo.Adapt<Api.ContactInfo>(),
                    errors: new[]
                    {
                        new Error(
                            "VERSION_CONFLICT",
                            "The customer has been modified since last viewed.",
                            $"Customer number {request.CustomerNumber} was last viewed with version {request.Version}, but is currently at version {customer.Version}.")
                    });
                return response;
            }

            // TODO: Resolve address with online service

            customer.UpdateContactInfo(request.NewContactInfo.Adapt<ContactInfo>());
            //customer.Version += 1;

            _boundedContext.Customers.Update(customer);
            await _boundedContext.SaveChangesAsync(cancellationToken);

            return new Response<Api.ContactInfo>(true, customer.ContactInfo.Adapt<Api.ContactInfo>());
        }
        catch (Exception ex)
        {
            return new Response<Api.ContactInfo>(false, errors: new[]
            {
                new Error(
                    "ERROR",
                    ex.Message,
                    ex.ToString())
            });
        }
    }
}