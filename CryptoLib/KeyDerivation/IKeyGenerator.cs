namespace CryptoLib.KeyDerivation;

public interface IKeyGenerator
{
    byte[] Generate(int keyBytes);
}