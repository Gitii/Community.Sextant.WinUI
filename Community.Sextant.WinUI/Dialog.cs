using Microsoft.UI.Xaml.Controls;

namespace Community.Sextant.WinUI;

/// <inheritdoc />
public class Dialog : IDialog
{
    private readonly ContentDialog _dialog;

    public Dialog(ContentDialog dialog)
    {
        _dialog = dialog;
    }

    /// <inheritdoc />
    public void Show()
    {
        _ = _dialog.ShowAsync();
    }

    /// <inheritdoc />
    public void Hide()
    {
        _dialog.Hide();
    }
}
