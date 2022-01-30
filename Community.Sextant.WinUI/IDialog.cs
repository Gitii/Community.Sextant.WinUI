using Microsoft.UI.Xaml.Controls;

namespace Community.Sextant.WinUI;

/// <summary>
/// A simple wrapper for a <seealso cref="ContentDialog"/>.
/// </summary>
public interface IDialog
{
    /// <inheritdoc cref="ContentDialog.ShowAsync()"/>
    public void Show();

    /// <inheritdoc cref="ContentDialog.Hide"/>
    public void Hide();
}
