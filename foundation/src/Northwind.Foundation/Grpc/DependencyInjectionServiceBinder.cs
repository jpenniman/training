using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;

namespace Northwind.Foundation.Grpc;

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