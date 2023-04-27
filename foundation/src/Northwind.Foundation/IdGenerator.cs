using System.Security.Cryptography;

namespace Northwind.Foundation;

/// <summary>
/// Generates unique alphanumeric values.
/// </summary>
public static class IdGenerator
{
    static readonly char[] _allowedCharacters = "ABCDEEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    /// <summary>
    /// Generates a new id of the specified length.
    /// </summary>
    /// <param name="length">The desired length of the generated value.</param>
    /// <returns>A unique random alphanumeric value.</returns>
    public static string Generate(int length)
    {
        var data = new byte[4 * length];
        using var gen = RandomNumberGenerator.Create();
        gen.GetBytes(data);
        var customerNo = new char[length];
        for (int i = 0; i < length; i++)
        {
            var rnd = BitConverter.ToInt32(data, i * 4);
            var idx = Math.Abs(rnd % _allowedCharacters.Length);

            customerNo[i] = _allowedCharacters[idx];
        }

        return new string(customerNo);
    }
}