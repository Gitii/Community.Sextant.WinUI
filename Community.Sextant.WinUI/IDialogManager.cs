using Microsoft.UI.Xaml;

namespace Community.Sextant.WinUI;

/// <summary>
/// A simple interface for creating and managing dialogs.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    /// Determines whether any dialogs are currently shown in the window.
    /// </summary>
    bool AreModalDialogsActive { get; }

    /// <summary>
    /// Creates a dialog and returns it.
    /// </summary>
    /// <param name="content">The content that will be shown in the dialog.</param>
    /// <returns>The instance to the created dialog.</returns>
    IDialog Create(object content);

    /// <summary>
    /// Sets the reference to the window.
    /// </summary>
    /// <param name="window">The window reference.</param>
    void SetWindow(Window window);
}
