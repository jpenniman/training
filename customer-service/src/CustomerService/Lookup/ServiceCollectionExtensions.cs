using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Api.Lookup;

namespace Northwind.CustomerService.Lookup;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddCustomerLookup(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<CustomerLookupBoundedContext>(options);
        services.AddScoped<ICustomerLookup, CustomerLookupService>();        
        return services;
    }
    
    public static WebApplicationBuilder AddCustomerLookup(this WebApplicationBuilder builder, Action<DbContextOptionsBuilder> options)
    {
        var provider = builder.Configuration.GetValue<string>("CustomerLookup:Provider") ?? "postgres";
        var connectionString = builder.Configuration.GetValue<string>("CustomerLookup:ConnectionString");
        builder.Services.AddCustomerLookup(options);
        return builder;
    }

    public static  IEndpointRouteBuilder MapCustomerLookupGrpc(this  IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGrpcService<ICustomerLookup>();
        return endpointRouteBuilder;
    }
}

