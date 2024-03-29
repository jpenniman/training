namespace Northwind.CustomerService.Api;

public sealed class OnboardCustomerRequest
{
    public OnboardCustomerRequest(string companyName, Address address, ContactInfo contactInfo)
    {
        CompanyName = companyName;
        Address = address;
        ContactInfo = contactInfo;
    }

    public string CompanyName { get; }
    
    public Address Address { get; }

    public ContactInfo ContactInfo { get; }
}