using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api;

namespace Northwind.CustomerService.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerService(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        return services;
    }
}