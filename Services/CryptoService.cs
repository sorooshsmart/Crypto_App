using CryptoLib.Factories;
using CryptoLib.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CryptoApp.Services;

/// <summary>
/// Main service for managing file encryption and decryption operations.
/// This service uses CryptoFactory to create appropriate cipher providers.
/// </summary>
public class CryptoService
{
    public CryptoService() { }

    /// <summary>
    /// Encrypt a byte array in memory.
    /// </summary>
    public (byte[] Cipher, byte[] IV) EncryptToMemory(byte[] plaintext, byte[] key, CryptoSettings settings, string algorithmName, byte[]? ivProvided = null)
    {
        var provider = CryptoFactory.CreateCipherProvider(algorithmName);
        
        var iv = ivProvided ?? RandomNumberGenerator.GetBytes(provider.IVLength); 
        
        var cipher = provider.Encrypt(plaintext, key, iv, settings);
        return (cipher, iv);
    }

    /// <summary>
    /// Encrypt a file and save it with metadata header.
    /// File structure: [4 bytes header length] + [JSON header] + [encrypted text]
    /// </summary>
    public void EncryptFile(string inputPath, string outputPath, byte[] key, CryptoSettings settings, string algorithmName, byte[]? ivProvided = null, byte[]? saltForKey = null, int kdfIterations = 100_000)
    {
        var provider = CryptoFactory.CreateCipherProvider(algorithmName);
        var plaintext = File.ReadAllBytes(inputPath);
        
        var iv = ivProvided ?? RandomNumberGenerator.GetBytes(provider.IVLength); 
        var cipher = provider.Encrypt(plaintext, key, iv, settings);

        // Create JSON header
        var header = new
        {
            algorithm = algorithmName,
            mode = settings.Mode.ToString(),
            iv = Convert.ToBase64String(iv),
            salt = saltForKey != null ? Convert.ToBase64String(saltForKey) : null,
            kdfIterations = saltForKey != null ? kdfIterations : (int?)null
        };

        var headerJson = JsonSerializer.Serialize(header);
        var headerBytes = Encoding.UTF8.GetBytes(headerJson);
        var headerLen = BitConverter.GetBytes(headerBytes.Length);

        using var fs = File.Create(outputPath);
        fs.Write(headerLen, 0, headerLen.Length);
        fs.Write(headerBytes, 0, headerBytes.Length);
        fs.Write(cipher, 0, cipher.Length);
    }

    /// <summary>
    /// Decrypt an encrypted file by reading metadata from header.
    /// </summary>
    public byte[] DecryptFileToMemory(string inputPath, byte[] key)
    {
        var all = File.ReadAllBytes(inputPath);

        if (all.Length < 4) throw new InvalidOperationException("File too small or missing header length.");

        // 1. Read header length
        var headerLen = BitConverter.ToInt32(all, 0);
        if (headerLen <= 0 || all.Length < 4 + headerLen) throw new InvalidOperationException("Invalid header length.");

        // 2. Read JSON header
        var headerBytes = new byte[headerLen];
        Buffer.BlockCopy(all, 4, headerBytes, 0, headerLen);
        var headerJson = Encoding.UTF8.GetString(headerBytes);

        // 3. Parse JSON and extract settings
        using var headerDoc = JsonDocument.Parse(headerJson);
        var root = headerDoc.RootElement;

        var algorithmName = root.GetProperty("algorithm").GetString() ?? throw new InvalidOperationException("Header missing algorithm");
        var modeString = root.GetProperty("mode").GetString() ?? throw new InvalidOperationException("Header missing mode");
        
        if (!Enum.TryParse<CipherMode>(modeString, true, out var mode))
        {
            throw new InvalidOperationException($"Unsupported cipher mode: {modeString}");
        }

        var ivBase64 = root.GetProperty("iv").GetString() ?? throw new InvalidOperationException("Header missing iv");
        var iv = Convert.FromBase64String(ivBase64);

        // 4. Extract encrypted text
        var cipher = new byte[all.Length - 4 - headerLen];
        Buffer.BlockCopy(all, 4 + headerLen, cipher, 0, cipher.Length);

        // 5. Create Provider and decrypt
        var provider = CryptoFactory.CreateCipherProvider(algorithmName);
        var settings = new CryptoSettings { Mode = mode }; 
        
        var plain = provider.Decrypt(cipher, key, iv, settings);
        return plain;
    }
}