using Northwind.Foundation;

namespace Northwind.CustomerService.Domain;

// DDD Value Type
// CustomerNumber is immutable
readonly struct CustomerNumber
{
    const int SIZE = 5;
    
    // CustomerNumber is immutable
    readonly string _value;
    
    CustomerNumber(string value) => _value = value;
    
    /// <summary>
    /// Parses the input string to create a new instance of a Customer Number.
    /// </summary>
    /// <param name="customerNo">The customer number to parse.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static CustomerNumber Parse(string customerNo)
    {
        if (string.IsNullOrWhiteSpace(customerNo))
            throw new ArgumentException("The value to parse cannot be blank", nameof(customerNo));
        
        if (customerNo.Length != SIZE)
            throw new ArgumentException($"Customer Numbers but be exactly {SIZE} alphanumeric characters in length", nameof(customerNo));

        if (customerNo.Except(IdGenerator.ALLOWED_CHARACTERS).Any())
            throw new ArgumentException("Invalid characters found.", nameof(customerNo));
        
        return new CustomerNumber(customerNo);
    }

    /// <summary>
    /// Generate a new unique CustomerNumber.
    /// </summary>
    /// <returns></returns>
    public static CustomerNumber New() => new CustomerNumber(IdGenerator.Generate(SIZE));

    ///<inheritDoc />
    public override string ToString() => _value;
    
    ///<exclude />
    public static implicit operator string(CustomerNumber right) => right._value; // string s = c.CustomerNumber;
    ///<exclude />
    public static explicit operator CustomerNumber(string right) => Parse(right); // c.CustomerNumber = s;
}