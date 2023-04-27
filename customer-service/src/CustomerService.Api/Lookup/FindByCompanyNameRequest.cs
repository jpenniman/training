using System.Runtime.Serialization;

namespace Northwind.CustomerService.Api.Lookup;

[DataContract]
public class FindByCompanyNameRequest
{
    FindByCompanyNameRequest() : this(null!)
    { }
    
    public FindByCompanyNameRequest(string partialCompanyName)
    {
        PartialCompanyName = partialCompanyName;
    }

    [DataMember(Order = 1)] public string PartialCompanyName { get; set; }

    [DataMember(Order = 2)] public int Skip { get; set; } = 0;

    [DataMember(Order = 3)] public int Take { get; set; } = 100;
}