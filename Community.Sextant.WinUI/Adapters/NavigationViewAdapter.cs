using System.Reactive;
using System.Reactive.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Community.Sextant.WinUI.Adapters;

public class NavigationViewAdapter : FrameNavigationViewAdapter
{
    private readonly NavigationView _navigationView;

    public NavigationViewAdapter(Frame frame, Window window, NavigationView navigationView)
        : base(frame, window)
    {
        _navigationView = navigationView;

        BackRequested = Observable
            .FromEvent<
                TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs>,
                NavigationViewBackRequestedEventArgs
            >(
                (x) => (_, y) => x(y),
                handler => navigationView.BackRequested += handler,
                handler => navigationView.BackRequested -= handler
            )
            .Select((args) => Unit.Default);
    }

    public override bool IsBackButtonVisible
    {
        get { return _navigationView.IsBackEnabled; }
        set { _navigationView.IsBackEnabled = value; }
    }
}
