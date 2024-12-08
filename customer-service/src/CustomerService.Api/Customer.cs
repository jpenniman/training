using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

[DataContract]
public sealed class Customer
{
    [DataMember(Order = 1)]
    public string CustomerNumber { get; set; }
    
    [DataMember(Order = 2)]
    public string CompanyName { get; set; }
    
    [DataMember(Order = 3)]
    public ContactInfo? ContactInfo { get; set; }
    
    [DataMember(Order = 4)]
    public Address? Address { get; set; } 
    
    [DataMember(Order = 5)]
    public long Version { get; set; }
}