using System.Security.Cryptography;

namespace CryptoLib.KeyDerivation;

public class RandomKeyGenerator : IKeyGenerator
{
    public byte[] Generate(int keyBytes)
    {
        var key = new byte[keyBytes];
        RandomNumberGenerator.Fill(key);
        return key;
    }
}