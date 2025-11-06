using CryptoLib.Models;

namespace CryptoLib.Algorithms;

public interface ICipherProvider
{
    /// <summary>
    /// IV length for the cipher (16 bytes for AES, 8 bytes for DES/3DES)
    /// </summary>
    int IVLength { get; } 
    
    byte[] Encrypt(byte[] plaintext, byte[] key, byte[] iv, CryptoSettings settings);
    byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv, CryptoSettings settings);
}