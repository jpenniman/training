namespace Northwind.CustomerService.Api.Lookup;

/// <summary>
/// Inputs for looking up a customer by their partial company name.
/// </summary>
/// <param name="PartialCompanyName">Partial name of the company.</param>
/// <param name="Skip">For pagination, the number of results to skip. Default: 0</param>
/// <param name="Take">For pagination, the maximum number of results to return. Default: 100</param>
public sealed record FindByCompanyNameRequest(string PartialCompanyName, int Skip = 0, int Take = 100);
