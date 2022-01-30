using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace Community.Sextant.WinUI.Adapters;

public class FrameNavigationViewAdapter : INavigationViewAdapter
{
    private readonly Frame _frame;

    public FrameNavigationViewAdapter(Frame frame, Window window)
    {
        Window = window;
        _frame = frame;

        Navigated = Observable
            .FromEvent<NavigatedEventHandler, NavigationEventArgs>(
                handler => (_, e) => handler(e),
                x => _frame.Navigated += x,
                x => _frame.Navigated -= x
            )
            .Select((args) => new ManagedNavigationEventArgs(args));

        BackRequested = Observable.Empty<Unit>();
    }

    public IObservable<INavigationEventArgs> Navigated { get; }
    public IObservable<Unit> BackRequested { get; protected set; }
    public virtual bool IsBackButtonVisible { get; set; } = false;
    public bool CanGoBack => _frame.CanGoBack;
    public object? Content => _frame.Content;

    public void GoBack(bool animated)
    {
        _frame.GoBack(
            animated ? new SlideNavigationTransitionInfo() : new SuppressNavigationTransitionInfo()
        );
    }

    public int BackStackDepth => _frame.BackStackDepth;

    public void Pop()
    {
        _frame.BackStack.RemoveAt(_frame.BackStack.Count - 1);
    }

    public void Navigate(Type viewType, bool animate)
    {
        _frame.Navigate(
            viewType,
            null,
            animate
              ? new EntranceNavigationTransitionInfo()
              : new SuppressNavigationTransitionInfo()
        );
    }

    public void ClearStack()
    {
        _frame.BackStack.Clear();
    }

    public Window Window { get; }
}
