using System.Reactive;
using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Community.Sextant.WinUI.Adapters;

public class BackButtonViewAdapter : FrameNavigationViewAdapter
{
    private readonly Button _backButton;

    public BackButtonViewAdapter(Frame frame, Window window, Button backButton)
        : base(frame, window)
    {
        _backButton = backButton;

        BackRequested = Observable
            .FromEvent<RoutedEventHandler, RoutedEventArgs>(
                (x) => (_, y) => x(y),
                handler => backButton.Click += handler,
                handler => backButton.Click -= handler
            )
            .Select((args) => Unit.Default);
    }

    public override bool IsBackButtonVisible
    {
        get { return _backButton.IsEnabled; }
        set { _backButton.IsEnabled = value; }
    }
}
