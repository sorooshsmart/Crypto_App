using CryptoApp.Services;
using CryptoLib.KeyDerivation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace CryptoApp.ViewModels;

public class DecryptViewModel : ViewModelBase
{
    private readonly CryptoService _cryptoService;
    private readonly PasswordKeyGenerator _passwordKeyGenerator = new();
    private readonly FileKeyGenerator _fileKeyGenerator = new();

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

    private string _selectedKeySource = "Password";
    public string SelectedKeySource
    {
        get => _selectedKeySource;
        set
        {
            if (SetProperty(ref _selectedKeySource, value))
            {
                OnPropertyChanged(nameof(IsPasswordKeySource));
                OnPropertyChanged(nameof(IsFileKeySource));
                DecryptCommand.RaiseCanExecuteChanged(); // +
            }
        }
    }

    public bool IsPasswordKeySource => SelectedKeySource == "Password";
    public bool IsFileKeySource => SelectedKeySource == "File";

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                DecryptCommand.RaiseCanExecuteChanged(); // +
            }
        }
    }

    private string _keyFilePath = string.Empty;
    public string KeyFilePath
    {
        get => _keyFilePath;
        set
        {
            if (SetProperty(ref _keyFilePath, value))
            {
                DecryptCommand.RaiseCanExecuteChanged(); // +
            }
        }
    }

    private int _keyLength = 32;
    public int KeyLength
    {
        get => _keyLength;
        set => SetProperty(ref _keyLength, value);
    }

    private string _statusMessage = "Ready to decrypt";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // --- Commands ---

    public DelegateCommand BrowseInputFileCommand { get; }
    public DelegateCommand BrowseOutputFileCommand { get; }
    public DelegateCommand BrowseKeyFileCommand { get; }
    public DelegateCommand DecryptCommand { get; }

    // --- Collections for UI ---

    public List<string> KeySources { get; } = new() { "Password", "File" };

    // --- Constructor ---

    public DecryptViewModel(CryptoService cryptoService)
    {
        _cryptoService = cryptoService;

        BrowseInputFileCommand = new DelegateCommand(_ => BrowseInputFile());
        BrowseOutputFileCommand = new DelegateCommand(_ => BrowseOutputFile());
        BrowseKeyFileCommand = new DelegateCommand(_ => BrowseKeyFile());
        DecryptCommand = new DelegateCommand(_ => DecryptFile(), _ => CanDecrypt());
    }

    // --- Methods ---

    private void BrowseInputFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Encrypted Files (*.enc)|*.enc|All Files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            InputFilePath = dialog.FileName;
            OutputFilePath = InputFilePath.EndsWith(".enc", StringComparison.OrdinalIgnoreCase)
                ? InputFilePath.Substring(0, InputFilePath.Length - 4)
                : InputFilePath + ".decrypted";
        }
        DecryptCommand.RaiseCanExecuteChanged();
    }

    private void BrowseOutputFile()
    {
        var dialog = new SaveFileDialog
        {
            FileName = OutputFilePath,
            Filter = "All Files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            OutputFilePath = dialog.FileName;
        }
        DecryptCommand.RaiseCanExecuteChanged();
    }

    private void BrowseKeyFile()
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            KeyFilePath = dialog.FileName;
        }
        DecryptCommand.RaiseCanExecuteChanged();
    }

    private bool CanDecrypt()
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
            "Password" => _passwordKeyGenerator.DeriveDeterministicFromPassword(Password, KeyLength),
            "File" => _fileKeyGenerator.GenerateFromFile(KeyFilePath, KeyLength),
            _ => throw new InvalidOperationException("Invalid key source selected.")
        };
    }

    private void DecryptFile()
    {
        if (!CanDecrypt())
        {
            StatusMessage = "Error: Invalid decryption inputs.";
            return;
        }

        try
        {
            StatusMessage = "Generating decryption key...";
            byte[] key = GetKey();

            StatusMessage = "Decrypting file...";

            byte[] plainBytes = _cryptoService.DecryptFileToMemory(InputFilePath, key);

            File.WriteAllBytes(OutputFilePath, plainBytes);

            StatusMessage = $"✓ Decryption successful! File saved to {OutputFilePath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Decryption error: {ex.Message}";
            MessageBox.Show($"Error: {ex.Message}", "Decryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            DecryptCommand.RaiseCanExecuteChanged();
        }
    }
}