namespace Northwind.CustomerService.Domain;

// DDD Value Type
sealed class ContactInfo : IEquatable<ContactInfo>
{
    public ContactInfo(string phoneNumber, string? name = null, string? title = null, string? faxNumber = null)
    {
        Name = name;
        Title = title;
        PhoneNumber = phoneNumber;
        FaxNumber = faxNumber;
    }

    public string? Name { get; }
    public string? Title { get;}
    public string PhoneNumber { get; }
    public string? FaxNumber { get; }

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