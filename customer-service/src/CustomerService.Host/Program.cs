using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Lookup;
using Northwind.Foundation.Grpc;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args)
    .AddGrpcServer()
    .AddManagementEndpoints();

// Application Services
builder.AddCustomerLookup(
    dbOptions => dbOptions.UseNpgsql(
        builder.Configuration.GetConnectionString("CustomerLookup"),
        pg => pg.EnableRetryOnFailure()));

var app = builder.Build();

app.UseRouting();

// gRPC Schema Reflection Endpoint
app.MapCodeFirstGrpcReflectionService();

// Application Endpoints
app.MapCustomerLookupGrpc();

app.Run();
