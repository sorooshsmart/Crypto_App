using CryptoLib.Models;
using System.Security.Cryptography;

namespace CryptoLib.Algorithms;

public class TripleDesProvider : ICipherProvider
{
    public int IVLength => 8; // 3DES uses 8-byte IV

    public byte[] Encrypt(byte[] plaintext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var tdes = TripleDES.Create();
        tdes.Key = key;
        tdes.Mode = settings.Mode;
        tdes.Padding = settings.Padding;

        if (iv == null || iv.Length == 0)
        {
            tdes.GenerateIV();
            iv = tdes.IV;
        }
        else
        {
            tdes.IV = iv;
        }

        using var encryptor = tdes.CreateEncryptor();
        return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
    }

    public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var tdes = TripleDES.Create();
        tdes.Key = key;
        tdes.IV = iv;
        tdes.Mode = settings.Mode;
        tdes.Padding = settings.Padding;

        using var decryptor = tdes.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }
}