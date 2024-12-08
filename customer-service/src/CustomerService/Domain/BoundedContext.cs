using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Northwind.CustomerService.Domain;

sealed class BoundedContext : DbContext
{
    public BoundedContext(DbContextOptions<BoundedContext> options) : base(options)
    { }

    // Customer is an Aggregate Root, so it gets a DbSet.
    public DbSet<Customer> Customers { get; private set; } = null!; //set by Entity Framework

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("customer_service");

        var customer = modelBuilder.Entity<Customer>().ToTable("customers");
        customer.HasKey(c => c.CustomerNumber);

        customer.Property(c => c.CustomerNumber)
            .HasColumnName("customer_id")
            .ValueGeneratedNever()
            .HasConversion(
                domainValue => domainValue.ToString(), 
                dbValue => CustomerNumber.Parse(dbValue));

        customer.Property(c => c.CompanyName);
        customer.Property(c => c.Version).IsConcurrencyToken();
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
