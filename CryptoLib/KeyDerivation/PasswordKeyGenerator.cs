using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.KeyDerivation;

public class PasswordKeyGenerator
{
    /// <summary>
    /// Deterministic derivation: salt derived from password (SHA256 of password bytes),
    /// then PBKDF2 applied. This makes key reproducible every time the same password is used.
    /// Note: this is weaker than using a random salt, but allows password-based decryption.
    /// </summary>
    public byte[] DeriveDeterministicFromPassword(string password, int keyBytes = 32, int iterations = 100_000)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (keyBytes <= 0) throw new ArgumentOutOfRangeException(nameof(keyBytes));

        // deterministic salt = SHA256(password)
        var pwdBytes = Encoding.UTF8.GetBytes(password);
        var salt = SHA256.HashData(pwdBytes); // 32 bytes

        using var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return kdf.GetBytes(keyBytes);
    }

    /// <summary>
    /// The previous secure method kept here for reference (not used when deterministic behavior requested).
    /// </summary>
    public byte[] GenerateKey_WithNewSalt(string password, int keyBytes, out byte[] salt, int iterations = 100_000)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        salt = RandomNumberGenerator.GetBytes(16);
        using var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return kdf.GetBytes(keyBytes);
    }
}