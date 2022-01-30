using System;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace Community.Sextant.WinUI.Adapters;

internal class ManagedNavigationEventArgs : INavigationEventArgs
{
    private readonly NavigationEventArgs _args;

    public ManagedNavigationEventArgs(NavigationEventArgs args)
    {
        _args = args;
    }

    public object Content => _args.Content;

    public NavigationMode NavigationMode => _args.NavigationMode;

    public NavigationTransitionInfo NavigationTransitionInfo => _args.NavigationTransitionInfo;

    public object Parameter => _args.Parameter;

    public Type SourcePageType => _args.SourcePageType;

    public Uri Uri
    {
        get => _args.Uri;
        set => _args.Uri = value;
    }
}
