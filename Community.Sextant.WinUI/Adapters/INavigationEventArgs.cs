using System;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace Community.Sextant.WinUI.Adapters;

/// <inheritdoc cref="NavigationEventArgs"/>
public interface INavigationEventArgs
{
    /// <inheritdoc cref="NavigationEventArgs.Content"/>
    object Content { get; }

    /// <inheritdoc cref="NavigationEventArgs.NavigationMode"/>
    NavigationMode NavigationMode { get; }

    /// <inheritdoc cref="NavigationEventArgs.NavigationTransitionInfo"/>
    NavigationTransitionInfo NavigationTransitionInfo { get; }

    /// <inheritdoc cref="NavigationEventArgs.Parameter"/>
    object Parameter { get; }

    /// <inheritdoc cref="NavigationEventArgs.SourcePageType"/>
    Type SourcePageType { get; }

    /// <inheritdoc cref="NavigationEventArgs.Uri"/>
    Uri Uri { get; set; }
}
