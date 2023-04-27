using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api.Lookup;

/// <exclude />
[DataContract]
public class FindByCustomerNoRequest
{
    // Required by serializers.
    FindByCustomerNoRequest() : this(null!)
    { }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="customerNo">The customer number to search for.</param>
    public FindByCustomerNoRequest(string customerNo)
    {
        CustomerNo = customerNo;
    }

    /// <summary>
    /// The customer number to search for.
    /// </summary>
    [DataMember(Order = 1)]
    public string CustomerNo { get; set; }
}