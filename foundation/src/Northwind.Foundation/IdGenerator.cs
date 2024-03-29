using System.Security.Cryptography;

namespace Northwind.Foundation;

/// <summary>
/// Generates unique alphanumeric values.
/// </summary>
public static class IdGenerator
{
    public static readonly char[] ALLOWED_CHARACTERS = "ABCDEEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

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
        var id = new char[length];
        for (int i = 0; i < length; i++)
        {
            var rnd = BitConverter.ToInt32(data, i * 4);
            var idx = Math.Abs(rnd % ALLOWED_CHARACTERS.Length);

            id[i] = ALLOWED_CHARACTERS[idx];
        }

        return new string(id);
    }
}