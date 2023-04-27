namespace Northwind.CustomerService.Api;

public class OnboardCustomerRequest
{
    public OnboardCustomerRequest(string companyName)
    {
        CompanyName = companyName;
    }

    public string CompanyName { get; set; }
}