using CryptoApp.Services;

namespace CryptoApp.ViewModels;

/// <summary>
/// Main ViewModel that holds both Encrypt and Decrypt ViewModels.
/// </summary>
public class MainViewModel : ViewModelBase
{
    public EncryptViewModel EncryptVM { get; }
    public DecryptViewModel DecryptVM { get; }

    public MainViewModel()
    {
        // CryptoService is created once and injected into both ViewModels.
        var cryptoService = new CryptoService();
        
        EncryptVM = new EncryptViewModel(cryptoService);
        DecryptVM = new DecryptViewModel(cryptoService);
    }
}