namespace Northwind.CustomerService.Api.Lookup;

/// <summary>
/// Customer search results.
/// </summary>
/// <param name="CustomerNumber">The customer number represented as a string.</param>
/// <param name="CompanyName">The name of the company.</param>
/// <param name="Country">The country the customer is located in.</param>
/// <param name="Phone">The primary phone number of for the customer.</param>
public sealed record CustomerLookupResult(string CustomerNumber, string CompanyName, string? Country, string? Phone);