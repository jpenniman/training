using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

[DataContract]
public sealed class ContactInfo
{
    // Required for serializers.
    ContactInfo() : this(null!)
    { }

    public ContactInfo(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    [DataMember(Order = 1)]
    public string? Name { get; set; }

    [DataMember(Order = 2)]
    public string? Title { get; set; }

    [DataMember(Order = 3)]
    public string PhoneNumber { get; set; }

    [DataMember(Order = 4)]
    public string? FaxNumber { get; set; }
}