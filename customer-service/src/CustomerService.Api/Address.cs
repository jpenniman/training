using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

[DataContract]
public sealed class Address
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
}