using System.Security.Cryptography;

namespace BusinessLogicLayer.Helper;

public class SHA256Helper
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;
    private const char Delimiter = ';';

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithm, KeySize);

        return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }
    
    public static bool verifyPassword(string passwordInput, string passwordHashed)
    {
        var elements = passwordHashed.Split(Delimiter);
        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);
        
        var hashInput = Rfc2898DeriveBytes.Pbkdf2(passwordInput, salt, Iterations, _hashAlgorithm, KeySize);
        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}