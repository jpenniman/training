using Northwind.Foundation.Server;

namespace Northwind.CustomerService.Domain;

// DDD Entity/Aggregate Root
sealed class Customer : IEquatable<Customer>, IVersionable
{
    public static Customer Create(string companyName, Address address, ContactInfo contactInfo)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("A company name must be provided.", nameof(companyName));
        
        return new Customer(companyName, CustomerNumber.New())
        {
            Address = address,
            ContactInfo = contactInfo
        };
    }
    
    Customer(string companyName, CustomerNumber customerNumber)
    {
        CompanyName = companyName;
        CustomerNumber = customerNumber;
        Address = null!;
        ContactInfo = null!;
    }
    
    public CustomerNumber CustomerNumber { get; }
    public string CompanyName { get; set; }
    public ContactInfo ContactInfo { get; private set; }
    public Address Address { get; private set; } 
    public int Version { get; set; }

    public void ChangeAddress(Address newAddress)
    {
        Address = newAddress;
    }

    public void UpdateContactInfo(ContactInfo updatedContactInfo)
    {
        ContactInfo = updatedContactInfo;
    }
    
    public bool Equals(Customer? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals (this, other)) return true;
        return CustomerNumber == other.CustomerNumber;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Customer);
    }

    public override int GetHashCode()
    {
        return CustomerNumber.GetHashCode();
    }
}