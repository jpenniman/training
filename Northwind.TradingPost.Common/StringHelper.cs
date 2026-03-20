using System;
using System.Linq;

namespace Northwind.TradingPost.Common;

public static class StringHelper
{
    public static string RemoveExtraSpaces(string input)
    {
        return string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    public static bool ContainsIgnoreCase(string source, string value)
    {
        return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static string StripNonNumeric(string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }

    public static string StripNonAlpha(string input)
    {
        return new string(input.Where(char.IsLetter).ToArray());
    }

    public static bool IsNumeric(string input)
    {
        return input.All(char.IsDigit);
    }

    public static bool IsAlphanumeric(string input)
    {
        return input.All(char.IsLetterOrDigit);
    }

    public static string PadLeft(string input, int totalWidth, char paddingChar)
    {
        return input.PadLeft(totalWidth, paddingChar);
    }

    public static string PadRight(string input, int totalWidth, char paddingChar)
    {
        return input.PadRight(totalWidth, paddingChar);
    }

    public static string Left(string input, int length)
    {
        return input.Length <= length ? input : input.Substring(0, length);
    }

    public static string Right(string input, int length)
    {
        return input.Length <= length ? input : input.Substring(input.Length - length);
    }

    public static string EscapeSql(string input)
    {
        return input.Replace("'", "''");
    }
}