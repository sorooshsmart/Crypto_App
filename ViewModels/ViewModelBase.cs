using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoApp.ViewModels;

/// <summary>
/// Base class for all ViewModels implementing INotifyPropertyChanged.
/// This is essential for Data Binding in WPF.
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Method to invoke PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Method to set a property value and invoke OnPropertyChanged if value changed.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">New value.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>True if value changed, otherwise False.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}