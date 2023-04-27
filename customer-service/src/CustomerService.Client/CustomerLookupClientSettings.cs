namespace CustomerService.Client;

class CustomerLookupClientSettings
{
    public const string SectionName = "CustomerLookup";
    public Uri Uri { get; set; } = new Uri("http://localhost");
}