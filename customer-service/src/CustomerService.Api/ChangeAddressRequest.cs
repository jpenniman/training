using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

/// <summary>
/// Change Address Request
/// </summary>
[DataContract]
public sealed class ChangeAddressRequest
{
    // Required for serializers
    ChangeAddressRequest() : this(null!, null!, 0)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerNumber"></param>
    /// <param name="newAddress"></param>
    /// <param name="version"></param>
    public ChangeAddressRequest(string customerNumber, Address newAddress, long version)
    {
        CustomerNumber = customerNumber;
        NewAddress = newAddress;
        Version = version;
    }

    /// <summary>
    /// The customer to change the address for.
    /// </summary>
    [DataMember(Order = 1)]
    public string CustomerNumber { get; set; }
    
    /// <summary>
    /// The new address.
    /// </summary>
    [DataMember(Order = 2)]
    public Address NewAddress { get; set; }

    /// <summary>
    /// The current known version of the customer.
    /// </summary>
    [DataMember(Order = 3)]
    public long Version { get; set; }
}
