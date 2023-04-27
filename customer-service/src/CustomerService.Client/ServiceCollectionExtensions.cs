using CustomerService.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Northwind.CustomerService.Api.Lookup;
using Polly;
using ProtoBuf.Grpc.ClientFactory;

namespace Northwind.CustomerService.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddCustomerLookupClient(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions()
            .Configure<CustomerLookupClientSettings>(configuration.GetSection(CustomerLookupClientSettings.SectionName));       
        
        return services.AddCodeFirstGrpcClient<ICustomerLookup>((provider, options) =>
        {
            var settings = provider.GetRequiredService<IOptionsMonitor<CustomerLookupClientSettings>>();
            options.Address = settings.CurrentValue.Uri;
        }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
        {
            TimeSpan.FromMilliseconds(1),
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(100)
        }));
    }
}