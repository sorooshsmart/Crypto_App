using System.Security.Cryptography;

namespace CryptoLib.Models;

public class CryptoSettings
{
    public CipherMode Mode { get; set; } = CipherMode.CBC;
    public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
}