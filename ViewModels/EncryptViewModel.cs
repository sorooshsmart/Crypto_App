using CryptoApp.Services;
using CryptoLib.KeyDerivation;
using CryptoLib.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace CryptoApp.ViewModels;

public class EncryptViewModel : ViewModelBase
{
    private readonly CryptoService _cryptoService;
    private readonly PasswordKeyGenerator _passwordKeyGenerator = new();
    private readonly FileKeyGenerator _fileKeyGenerator = new();
    private readonly RandomKeyGenerator _randomKeyGenerator = new();

    // --- Properties ---

    private string _inputFilePath = string.Empty;
    public string InputFilePath
    {
        get => _inputFilePath;
        set => SetProperty(ref _inputFilePath, value);
    }

    private string _outputFilePath = string.Empty;
    public string OutputFilePath
    {
        get => _outputFilePath;
        set => SetProperty(ref _outputFilePath, value);
    }

    private string _selectedAlgorithm = "AES";
    public string SelectedAlgorithm
    {
        get => _selectedAlgorithm;
        set
        {
            if (SetProperty(ref _selectedAlgorithm, value))
            {
                UpdateKeyLength();
            }
        }
    }

    private string _selectedMode = "CBC";
    public string SelectedMode
    {
        get => _selectedMode;
        set => SetProperty(ref _selectedMode, value);
    }

    private string _selectedKeySource = "Random";
    public string SelectedKeySource
    {
        get => _selectedKeySource;
        set
        {
            if (SetProperty(ref _selectedKeySource, value))
            {
                OnPropertyChanged(nameof(IsPasswordKeySource));
                OnPropertyChanged(nameof(IsFileKeySource));
                OnPropertyChanged(nameof(IsRandomKeySource));
            }
        }
    }

    public bool IsPasswordKeySource => SelectedKeySource == "Password";
    public bool IsFileKeySource => SelectedKeySource == "File";
    public bool IsRandomKeySource => SelectedKeySource == "Random";

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _keyFilePath = string.Empty;
    public string KeyFilePath
    {
        get => _keyFilePath;
        set => SetProperty(ref _keyFilePath, value);
    }

    private int _keyLength = 32; // Default AES-256
    public int KeyLength
    {
        get => _keyLength;
        set => SetProperty(ref _keyLength, value);
    }

    private string _statusMessage = "Ready to encrypt";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // --- Commands ---

    public DelegateCommand BrowseInputFileCommand { get; }
    public DelegateCommand BrowseOutputFileCommand { get; }
    public DelegateCommand BrowseKeyFileCommand { get; }
    public DelegateCommand EncryptCommand { get; }

    // --- Collections for UI ---

    public List<string> Algorithms { get; } = new() { "AES", "DES", "3DES" };
    public List<string> Modes { get; } = new() { "CBC", "ECB" };
    public List<string> KeySources { get; } = new() { "Random", "Password", "File" };

    // --- Constructor ---

    public EncryptViewModel(CryptoService cryptoService)
    {
        _cryptoService = cryptoService;

        BrowseInputFileCommand = new DelegateCommand(_ => BrowseInputFile());
        BrowseOutputFileCommand = new DelegateCommand(_ => BrowseOutputFile());
        BrowseKeyFileCommand = new DelegateCommand(_ => BrowseKeyFile());
        EncryptCommand = new DelegateCommand(_ => EncryptFile(), _ => CanEncrypt());
    }

    // --- Methods ---

    private void UpdateKeyLength()
    {
        KeyLength = SelectedAlgorithm switch
        {
            "AES" => 32, // AES-256
            "DES" => 8,  // DES
            "3DES" => 24, // 3DES
            _ => 32
        };
        EncryptCommand.RaiseCanExecuteChanged();
    }

    private void BrowseInputFile()
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            InputFilePath = dialog.FileName;
            OutputFilePath = InputFilePath + ".enc";
        }
        EncryptCommand.RaiseCanExecuteChanged();
    }

    private void BrowseOutputFile()
    {
        var dialog = new SaveFileDialog
        {
            FileName = OutputFilePath,
            Filter = "Encrypted Files (*.enc)|*.enc|All Files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            OutputFilePath = dialog.FileName;
        }
        EncryptCommand.RaiseCanExecuteChanged();
    }

    private void BrowseKeyFile()
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            KeyFilePath = dialog.FileName;
        }
        EncryptCommand.RaiseCanExecuteChanged();
    }

    private bool CanEncrypt()
    {
        if (string.IsNullOrWhiteSpace(InputFilePath) || !File.Exists(InputFilePath)) return false;
        if (string.IsNullOrWhiteSpace(OutputFilePath)) return false;
        if (InputFilePath.Equals(OutputFilePath, StringComparison.OrdinalIgnoreCase)) return false;

        if (IsPasswordKeySource && string.IsNullOrWhiteSpace(Password)) return false;
        if (IsFileKeySource && (string.IsNullOrWhiteSpace(KeyFilePath) || !File.Exists(KeyFilePath))) return false;

        return true;
    }

    private byte[] GetKey()
    {
        return SelectedKeySource switch
        {
            "Random" => _randomKeyGenerator.Generate(KeyLength),
            "Password" => _passwordKeyGenerator.DeriveDeterministicFromPassword(Password, KeyLength),
            "File" => _fileKeyGenerator.GenerateFromFile(KeyFilePath, KeyLength),
            _ => throw new InvalidOperationException("Invalid key source selected.")
        };
    }

    private void EncryptFile()
    {
        if (!CanEncrypt())
        {
            StatusMessage = "Error: Invalid encryption inputs.";
            return;
        }

        try
        {
            StatusMessage = "Generating encryption key...";
            byte[] key = GetKey();

            byte[]? salt = SelectedKeySource switch
            {
                "Password" => SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(Password)),
                "File" => SHA256.HashData(File.ReadAllBytes(KeyFilePath)),
                _ => null
            };

            StatusMessage = $"Encrypting file with {SelectedAlgorithm}-{SelectedMode}...";

            var settings = new CryptoSettings
            {
                Mode = (CipherMode)Enum.Parse(typeof(CipherMode), SelectedMode),
                Padding = PaddingMode.PKCS7
            };

            _cryptoService.EncryptFile(
                InputFilePath,
                OutputFilePath,
                key,
                settings,
                SelectedAlgorithm,
                saltForKey: salt
            );

            StatusMessage = $"✓ Encryption successful! File saved to {OutputFilePath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Encryption error: {ex.Message}";
            MessageBox.Show($"Error: {ex.Message}", "Encryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EncryptCommand.RaiseCanExecuteChanged();
        }
    }
}