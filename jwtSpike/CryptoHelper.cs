using System.Security.Cryptography;

namespace jwtSpike;

public static class CryptoHelper
{
    private const int HashIterations = 10240;
    public static string? GeneratePasswordHash(string password)
    {
        var rnd = RandomNumberGenerator.GetBytes(16);
        using var crypto = new Rfc2898DeriveBytes(password, rnd, HashIterations, HashAlgorithmName.SHA256);
        return $"{Convert.ToBase64String(crypto.Salt)}|{crypto.IterationCount}|{Convert.ToBase64String(crypto.GetBytes(32))}|{crypto.HashAlgorithm}";
    }

    public static bool VerifyPassword(string hash, string password)
    {
        var hashParts = hash.Split('|');
        var salt = Convert.FromBase64String(hashParts[0]);
        var iterations = int.Parse(hashParts[1]);
        var origHash = hashParts[2];
        var algorithm = HashAlgorithmName.SHA256;

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, algorithm);
        var generatedHash = Convert.ToBase64String(pbkdf2.GetBytes(32));

        return  generatedHash == origHash;
    }
}