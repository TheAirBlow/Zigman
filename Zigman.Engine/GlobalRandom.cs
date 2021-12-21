namespace Zigman.Engine;

/// <summary>
/// Global random
/// </summary>
public class GlobalRandom
{
    /// <summary>
    /// Random instance
    /// </summary>
    public static Random Random = new Random();
    
    /// <summary>
    /// Generate a random string
    /// </summary>
    /// <param name="chars">Charset</param>
    /// <param name="length">Length</param>
    /// <returns>Random string</returns>
    public static string RandomString(string chars, int length)
        => new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
}