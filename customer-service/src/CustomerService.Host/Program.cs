using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Lookup;
using Northwind.Foundation.Server;

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

// Application Endpoints
app.MapCustomerLookupGrpc();

app.Run();
