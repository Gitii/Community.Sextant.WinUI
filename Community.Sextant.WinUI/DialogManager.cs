using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Community.Sextant.WinUI;

/// <inheritdoc />
public class DialogManager : IDialogManager
{
    private Window? _window;

    /// <inheritdoc />
    public void SetWindow(Window window)
    {
        _window = window;
    }

    /// <inheritdoc />
    public bool AreModalDialogsActive
    {
        get { return _window.GetOpenPopups().Any(x => x.Child is ContentDialog); }
    }

    /// <inheritdoc />
    public IDialog Create(object content)
    {
        if (content is not Page)
        {
            throw new ArgumentException(
                $"Content must be of type Page but it's of type '{content?.GetType().FullName ?? "null"}'. This indicates a wrong view type has been registered.",
                nameof(content)
            );
        }

        return new Dialog(
            new ContentDialog
            {
                FullSizeDesired = false,
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                Content = content,
                XamlRoot =
                    _window?.Content.XamlRoot
                    ?? throw new ArgumentNullException(
                        nameof(_window),
                        "Call DialogManager.SetWindow before using it."
                    )
            }
        );
    }
}
