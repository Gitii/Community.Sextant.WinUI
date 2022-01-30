using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

namespace Community.Sextant.WinUI.Adapters;

public class NullNavigationViewAdapter : INavigationViewAdapter
{
    public NullNavigationViewAdapter()
    {
        Navigated = Observable.Never<INavigationEventArgs>();
        BackRequested = Observable.Never<Unit>();
    }

    public IObservable<INavigationEventArgs> Navigated { get; }
    public IObservable<Unit> BackRequested { get; }
    public bool IsBackButtonVisible { get; set; }
    public bool CanGoBack { get; } = false;
    public object? Content { get; } = null;

    public void GoBack(bool animated)
    {
        throw new NotSupportedException();
    }

    public int BackStackDepth { get; } = 0;

    public void Pop()
    {
        throw new NotSupportedException();
    }

    public void Navigate(Type viewType, bool animate)
    {
        throw new NotSupportedException();
    }

    public void ClearStack()
    {
        throw new NotSupportedException();
    }

    public Window Window { get; } = null!;
}
