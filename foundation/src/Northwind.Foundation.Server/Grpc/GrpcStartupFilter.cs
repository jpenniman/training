using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ProtoBuf.Grpc.Server;

namespace Northwind.Foundation.Server;

sealed class GrpcStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            next(builder);
            builder.UseEndpoints(ep => ep.MapCodeFirstGrpcReflectionService());
        };
    }
}