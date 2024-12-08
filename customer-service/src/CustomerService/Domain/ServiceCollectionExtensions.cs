using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api;
using Northwind.Foundation.Server;

namespace Northwind.CustomerService.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerService(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddDbContext<BoundedContext>(builder =>
        {
            builder.AddInterceptors(new VersionIncrementInterceptor());
            options(builder);
        });
        return services;
    }
}