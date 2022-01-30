using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;

namespace Community.Sextant.WinUI;

public static class WindowExtensions
{
    /// <summary>
    /// Gets the handle of <paramref name="window"/>.
    /// If <paramref name="window"/> is <c>null</c>, an <seealso cref="ArgumentNullException"/> will be thrown.).
    /// </summary>
    /// <returns>The handle of the current window.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="window"/> is null.</exception>
    public static IntPtr GetHandle(this Window? window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(
                nameof(window),
                "The current window is null. Please set the current window first."
            );
        }

        return WindowNative.GetWindowHandle(window);
    }

    /// <summary>Retrieves a collection of all open popup controls from the target XamlRoot.</summary>
    /// <returns>The list of all open popups. If no popups are open or there is no window set, the list is empty.</returns>
    public static IReadOnlyList<Popup> GetOpenPopups(this Window? window)
    {
        if (window == null)
        {
            return Array.Empty<Popup>();
        }

        return VisualTreeHelper.GetOpenPopupsForXamlRoot(window.Content.XamlRoot);
    }
}
