using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CustomerService.Api.Lookup;
using Polly;
using Polly.Timeout;
using ProtoBuf.Grpc.ClientFactory;

namespace Northwind.CustomerService.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddCustomerLookupClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddCustomerLookupClient(configuration, null);
    }
    
    // For testing purposes only
    internal static IHttpClientBuilder AddCustomerLookupClient(this IServiceCollection services, IConfiguration configuration, HttpMessageHandler? httpHandler)
    {
        var configSection = configuration.GetSection(CustomerLookupClientSettings.SECTION_NAME);
        var settings = configSection.Get<CustomerLookupClientSettings>() ?? new CustomerLookupClientSettings();

        services
            .AddOptions()
            .Configure<CustomerLookupClientSettings>(configSection);
        
        return services.AddCodeFirstGrpcClient<ICustomerLookup>(options =>
            {
                options.Address = settings.Uri;
                if (httpHandler is not null)
                    options.ChannelOptionsActions.Add(c => c.HttpHandler = httpHandler);
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>()
                .AdvancedCircuitBreakerAsync(
                    settings.CircuitBreakerSettings.FailureThresholdPercentage, 
                    settings.CircuitBreakerSettings.SamplingWindow, 
                    settings.CircuitBreakerSettings.MinRequestsDuringSamplingWindow, 
                    settings.CircuitBreakerSettings.BreakDuration))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(settings.MaxRetryAttempts, ExponentialBackoffWithJitter))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(settings.Timeout));
    }

    static TimeSpan ExponentialBackoffWithJitter(int retryAttempt)
    {
        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential back-off
               + TimeSpan.FromMilliseconds(_jitter.Next(0, 100)); // plus some jitter
    }
    static readonly Random _jitter = new(); 

}
