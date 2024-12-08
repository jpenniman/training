namespace Northwind.CustomerService.Domain;

// DDD Value Type.
// As of EF Core 7, Owned ValueTypes must be classes.
// Struct support is in the backlog: https://github.com/dotnet/efcore/issues/9906
sealed class Address : IEquatable<Address>
{
    public Address(string country, string? street = null, string? city = null, string? stateOrProvince = null, string? postalCode = null)
    {
        Street = street;
        City = city;
        StateOrProvince = stateOrProvince;
        PostalCode = postalCode;
        Country = country;
    }

    public string? Street { get; }
    public string? City { get; }
    public string? StateOrProvince { get; }
    public string? PostalCode { get; }
    public string Country { get; }

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