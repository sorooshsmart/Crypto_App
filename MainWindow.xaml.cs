using System.Windows;
using System.Windows.Controls;

namespace CryptoApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EncryptPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel mainVm && sender is PasswordBox passwordBox)
        {
            mainVm.EncryptVM.Password = passwordBox.Password;
        }
    }

    private void DecryptPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel mainVm && sender is PasswordBox passwordBox)
        {
            mainVm.DecryptVM.Password = passwordBox.Password;
        }
    }
}