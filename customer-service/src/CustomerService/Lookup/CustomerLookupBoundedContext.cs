using Microsoft.EntityFrameworkCore;
using Northwind.CustomerService.Api.Lookup;

namespace Northwind.CustomerService.Lookup;

sealed class CustomerLookupBoundedContext : DbContext
{
    public CustomerLookupBoundedContext(DbContextOptions<CustomerLookupBoundedContext> options) : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Since this is a read-only context, disable entity change tracking to increase performance.
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("customer_service");

        var customerLookupResult = modelBuilder
            .Entity<CustomerLookupResult>()
            .ToView("customers");
        customerLookupResult.HasNoKey();
        customerLookupResult.Property(c => c.CustomerNumber).HasColumnName("customer_id");
        customerLookupResult.Property(c => c.CompanyName);
        customerLookupResult.Property(c => c.Country);
        customerLookupResult.Property(c => c.Phone);
        
        // Since the underlying table is the same table used by the Domain, we exclude it from Migrations here.
        var customer = modelBuilder
            .Entity<Api.Customer>()
            .ToTable("customers", t => t.ExcludeFromMigrations());
        customer.HasKey(c => c.CustomerNumber);

        customer.Property(c => c.CustomerNumber).HasColumnName("customer_id");
        customer.Property(c => c.CompanyName);
        customer.Property(c => c.Version);
        customer.OwnsOne(c => c.ContactInfo,
            contact =>
            {
                contact.Property(ci => ci.Name).HasColumnName("contact_name");
                contact.Property(ci => ci.Title).HasColumnName("contact_title");
                contact.Property(ci => ci.PhoneNumber).HasColumnName("phone");
                contact.Property(ci => ci.FaxNumber).HasColumnName("fax");
            });
        customer.OwnsOne(c => c.Address,
            address =>
            {
                address.Property(a => a.Street).HasColumnName("address");
                address.Property(a => a.City).HasColumnName("city");
                address.Property(a => a.StateOrProvince).HasColumnName("region");
                address.Property(a => a.PostalCode).HasColumnName("postal_code");
                address.Property(a => a.Country).HasColumnName("country");
            });
    }
}