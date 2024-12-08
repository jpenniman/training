using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api;

/// <summary>
/// Request to update a customer's contact information.
/// </summary>
[DataContract]
public class UpdateContactInfoRequest
{
    // Required for serializers.
    UpdateContactInfoRequest() : this (null!, null!)
    { }

    /// <summary>
    /// Creates a new request.
    /// </summary>
    /// <param name="customerNumber"></param>
    /// <param name="newContactInfo"></param>
    public UpdateContactInfoRequest(string customerNumber, ContactInfo newContactInfo)
    {
        CustomerNumber = customerNumber;
        NewContactInfo = newContactInfo;
    }

    /// <summary>
    /// The customer to update the contact information for.
    /// </summary>
    [DataMember(Order = 1)]
    public string CustomerNumber { get; set; }

    /// <summary>
    /// The new customer contact information.
    /// </summary>
    [DataMember(Order = 2)]
    public ContactInfo NewContactInfo { get; set; }
    
    /// <summary>
    /// The current known version of the customer.
    /// </summary>
    [DataMember(Order = 3)]
    public long Version { get; set; }
}