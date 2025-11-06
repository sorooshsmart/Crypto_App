using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoLib.KeyDerivation;

public class FileKeyGenerator
{
    /// <summary>
    /// Generate key from file: if keyBytes == 32 (AES-256) return SHA256(file) directly.
    /// If longer length needed, use PBKDF2 with salt derived from hash.
    /// </summary>
    public byte[] GenerateFromFile(string path, int keyBytes, int iterations = 100_000)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("Key file not found", path);
        if (keyBytes <= 0) throw new ArgumentOutOfRangeException(nameof(keyBytes));

        var content = File.ReadAllBytes(path);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(content); // 32 bytes

        // If exactly 32 bytes requested (common for AES-256), return directly
        if (keyBytes == hash.Length)
        {
            return hash;
        }

        // If length less than 32, truncate
        if (keyBytes < hash.Length)
        {
            var result = new byte[keyBytes];
            Array.Copy(hash, 0, result, 0, keyBytes);
            return result;
        }

        // If longer length requested, use PBKDF2.
        // For salt, use a derivative of hash (deterministic).
        var salt = sha.ComputeHash(hash); // 32-byte derived salt
        using var kdf = new Rfc2898DeriveBytes(hash, salt, iterations, HashAlgorithmName.SHA256);
        return kdf.GetBytes(keyBytes);
    }
}