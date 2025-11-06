using CryptoLib.Models;
using System.Security.Cryptography;

namespace CryptoLib.Algorithms;

public class AesProvider : ICipherProvider
{
    public int IVLength => 16; // AES uses 16-byte IV
    
    public byte[] Encrypt(byte[] plaintext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = settings.Mode;
        aes.Padding = settings.Padding;

        if (iv == null || iv.Length == 0)
        {
            aes.GenerateIV();
            iv = aes.IV;
        }
        else
        {
            aes.IV = iv;
        }

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
    }

    public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = settings.Mode;
        aes.Padding = settings.Padding;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }
}