using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

[DataContract]
public sealed class ContactInfo : IEquatable<ContactInfo>
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

    public bool Equals(ContactInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Title == other.Title && PhoneNumber == other.PhoneNumber && FaxNumber == other.FaxNumber;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ContactInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Title, PhoneNumber, FaxNumber);
    }
}