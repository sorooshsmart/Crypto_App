# 🔐 Manus Crypto Solution

A modern WPF application for secure file encryption and decryption using AES, DES, and 3DES algorithms.

## 📋 Features

- **Multiple Encryption Algorithms**: AES, DES, 3DES
- **Flexible Key Sources**: 
  - Random key generation
  - Password-based key derivation (PBKDF2)
  - File-based key generation
- **Cipher Modes**: CBC, ECB
- **Modern UI**: Dark theme with smooth animations
- **Easy to Use**: Intuitive tabbed interface for encryption and decryption

## 🏗️ Project Structure

```
CryptoApp/
├── CryptoApp.csproj                    # Project file
├── App.xaml                            # Application definition
├── App.xaml.cs                         # Application code-behind
├── MainWindow.xaml                     # Main window UI
├── MainWindow.xaml.cs                  # Main window code-behind
│
├── Styles/
│   └── Theme.xaml                      # Modern dark theme styles
│
├── ViewModels/
│   ├── ViewModelBase.cs                # Base ViewModel with INotifyPropertyChanged
│   ├── DelegateCommand.cs              # ICommand implementation
│   ├── MainViewModel.cs                # Main ViewModel
│   ├── EncryptViewModel.cs             # Encryption logic
│   └── DecryptViewModel.cs             # Decryption logic
│
├── Services/
│   └── CryptoService.cs                # Main cryptography service
│
└── CryptoLib/
    ├── Models/
    │   └── CryptoSettings.cs           # Encryption settings model
    │
    ├── Algorithms/
    │   ├── ICipherProvider.cs          # Cipher provider interface
    │   ├── AesProvider.cs              # AES implementation
    │   ├── DesProvider.cs              # DES implementation
    │   └── TripleDesProvider.cs        # 3DES implementation
    │
    ├── Factories/
    │   └── CryptoFactory.cs            # Algorithm factory
    │
    └── KeyDerivation/
        ├── IKeyGenerator.cs            # Key generator interface
        ├── RandomKeyGenerator.cs       # Random key generation
        ├── PasswordKeyGenerator.cs     # Password-based key derivation
        └── FileKeyGenerator.cs         # File-based key generation
```

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK

### Installation & Running

1. **Clone or download this repository**

2. **Open in Visual Studio**:
   - Open `CryptoApp.sln` in Visual Studio
   - Or open the `CryptoApp` folder

3. **Restore NuGet packages**:
   ```
   Visual Studio will automatically restore packages
   ```

4. **Build the solution**:
   - Press `Ctrl+Shift+B` or
   - Menu: Build → Build Solution

5. **Run the application**:
   - Press `F5` or
   - Menu: Debug → Start Debugging

## 📖 Usage

### Encrypting a File

1. Go to the **"🔒 Encrypt File"** tab
2. Click **"Browse..."** next to **Input File** and select your file
3. Choose an **Output File** location (default: `yourfile.enc`)
4. Select your desired:
   - **Algorithm** (AES, DES, or 3DES)
   - **Cipher Mode** (CBC or ECB)
   - **Key Source** (Random, Password, or File)
5. If using Password: Enter your password
6. If using File: Browse to select a key file
7. Click **"🔒 Encrypt File"**

### Decrypting a File

1. Go to the **"🔓 Decrypt File"** tab
2. Click **"Browse..."** next to **Encrypted File** and select your `.enc` file
3. Choose an **Output File** location
4. Select your **Key Source** (Password or File)
5. Enter the same password or select the same key file used for encryption
6. Click **"🔓 Decrypt File"**

## ⚠️ Important Notes

- **Random Key Encryption**: If you encrypt with a random key, you won't be able to decrypt the file later. Use Password or File key sources for recoverable encryption.
- **Password Security**: Use strong passwords for better security.
- **Key File Safety**: Keep your key files secure and backed up.
- **Algorithm Security**: AES is the most secure option. DES and 3DES are included for compatibility but are considered less secure.

## 🔧 Technical Details

### Architecture

- **MVVM Pattern**: Clean separation of concerns with ViewModels
- **Factory Pattern**: Dynamic algorithm selection via CryptoFactory
- **Strategy Pattern**: ICipherProvider interface for different algorithms
- **Dependency Injection**: Services injected into ViewModels

### File Format

Encrypted files have the following structure:
```
[4 bytes: header length]
[JSON header with metadata]
[Encrypted data]
```

The JSON header contains:
- Algorithm name
- Cipher mode
- Initialization Vector (IV)
- Salt (for password/file-based encryption)
- KDF iterations

### Key Derivation

- **Password**: Uses PBKDF2 with SHA256 (100,000 iterations)
- **File**: Uses SHA256 hash of file content
- **Random**: Cryptographically secure random bytes

## 📝 License

This project is open source and available for educational and commercial use.

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

## 📧 Contact

For questions or support, please open an issue in the repository.

---

**Built with ❤️ using .NET 8 and WPF**
