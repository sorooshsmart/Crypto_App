using CryptoLib.Algorithms;
using System;

namespace CryptoLib.Factories;

/// <summary>
/// Factory for creating ICipherProvider instances based on algorithm name.
/// This separates CryptoService from algorithm implementation details.
/// </summary>
public static class CryptoFactory
{
    public static ICipherProvider CreateCipherProvider(string algorithmName)
    {
        return algorithmName.ToUpperInvariant() switch
        {
            "AES" => new AesProvider(),
            "DES" => new DesProvider(),
            "3DES" => new TripleDesProvider(),
            _ => throw new ArgumentException($"Unsupported algorithm: {algorithmName}", nameof(algorithmName)),
        };
    }
}