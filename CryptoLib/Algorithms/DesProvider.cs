using CryptoLib.Models;
using System.Security.Cryptography;

namespace CryptoLib.Algorithms;

public class DesProvider : ICipherProvider
{
    public int IVLength => 8; // DES uses 8-byte IV

    public byte[] Encrypt(byte[] plaintext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var des = DES.Create();
        des.Key = key;
        des.Mode = settings.Mode;
        des.Padding = settings.Padding;

        if (iv == null || iv.Length == 0)
        {
            des.GenerateIV();
            iv = des.IV;
        }
        else
        {
            des.IV = iv;
        }

        using var encryptor = des.CreateEncryptor();
        return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
    }

    public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, CryptoSettings settings)
    {
        using var des = DES.Create();
        des.Key = key;
        des.IV = iv;
        des.Mode = settings.Mode;
        des.Padding = settings.Padding;

        using var decryptor = des.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }
}