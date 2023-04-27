using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;
using Steeltoe.Management.Endpoint;

namespace Northwind.Foundation.Grpc;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the required services for Protobuf-Net gRPC.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddGrpcServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddCodeFirstGrpc();
        builder.Services.AddCodeFirstGrpcReflection();
        builder.Services.AddSingleton(BinderConfiguration.Create(binder: new DependencyInjectionServiceBinder(builder.Services)));
        return builder;
    }

    /// <summary>
    /// Adds Northwind's standard management endpoints.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddManagementEndpoints(this WebApplicationBuilder builder)
    {
        builder
            .AddHealthActuator()
            .AddInfoActuator()
            .AddDbMigrationsActuator()
            .AddEnvActuator();

        return builder;
    }
}