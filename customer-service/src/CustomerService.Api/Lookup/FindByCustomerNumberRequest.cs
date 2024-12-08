namespace Northwind.CustomerService.Api.Lookup;

/// <summary>
/// Inputs for looking up a customer by their full customer number.
/// </summary>
/// <param name="CustomerNumber">The Customer's 5 character customer number.</param>
public sealed record FindByCustomerNumberRequest(string CustomerNumber);
