using System;

namespace Northwind.TradingPost.Common;

public static class ValidationHelper
{
    public static bool IsValidDate(DateTime? date)
    {
        return date.HasValue && date.Value > DateTime.MinValue && date.Value < DateTime.MaxValue;
    }

    public static bool IsValidPostalCode(string postalCode, string country)
    {
        if (country == "USA")
            return postalCode.Length == 5 || postalCode.Length == 9;
        if (country == "Canada")
            return postalCode.Length == 6;
        return true;
    }

    public static bool IsValidRegion(string region)
    {
        var validRegions = new[] { "BC", "CA", "CO", "CT", "DC", "FL", "GA", "ID", "IL", "IN", "KY", "LA", "MA", "MD", "MI", "MN", "MO", "NC", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "TX", "UT", "VA", "VT", "WA", "WI", "WY" };
        return validRegions.Contains(region);
    }

    public static bool IsValidPrice(decimal? price)
    {
        return price.HasValue && price.Value >= 0 && price.Value < 1000000;
    }

    public static bool IsValidQuantity(int? quantity)
    {
        return quantity.HasValue && quantity.Value > 0 && quantity.Value <= 9999;
    }

    public static bool IsValidDiscount(decimal? discount)
    {
        return discount.HasValue && discount.Value >= 0 && discount.Value <= 1;
    }

    public static bool IsValidEmployeeId(int? employeeId)
    {
        return employeeId.HasValue && employeeId.Value > 0;
    }

    public static bool IsValidShipperId(int? shipperId)
    {
        return shipperId.HasValue && shipperId.Value > 0;
    }
}