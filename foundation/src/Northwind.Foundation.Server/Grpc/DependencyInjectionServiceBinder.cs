using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;

namespace Northwind.Foundation.Server;

/// <summary>
/// Allows gRPC services to be resolved by interface instead of implementation.
/// </summary>
sealed class DependencyInjectionServiceBinder : ServiceBinder
{
    readonly IServiceCollection _services;

    public DependencyInjectionServiceBinder(IServiceCollection services)
    {
        _services = services;
    }

    public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
    {
        var resolvedServiceType = serviceType;
        if (serviceType.IsInterface)
            resolvedServiceType = _services.SingleOrDefault(x => x.ServiceType == serviceType)?.ImplementationType ?? serviceType;

        return base.GetMetadata(method, contractType, resolvedServiceType);
    }
}