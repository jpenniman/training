using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

[DataContract]
public sealed class Address : IEquatable<Address>
{
    // Required for serializers.
    Address() : this(null!)
    { }

    public Address(string country)
    {
        Country = country;
    }

    [DataMember(Order = 1)]
    public string? Street { get; set; }
    
    [DataMember(Order = 2)]
    public string? City { get; set; }
    
    [DataMember(Order = 3)]
    public string? StateOrProvince { get; set; }
    
    [DataMember(Order = 4)]
    public string? PostalCode { get; set; }

    [DataMember(Order = 5)] 
    public string Country { get; set; }

    public bool Equals(Address? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Street == other.Street && City == other.City && StateOrProvince == other.StateOrProvince && PostalCode == other.PostalCode && Country == other.Country;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Address other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, StateOrProvince, PostalCode, Country);
    }
}