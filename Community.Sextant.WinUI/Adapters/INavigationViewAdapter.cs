using System;
using System.Reactive;
using Microsoft.UI.Xaml;

namespace Community.Sextant.WinUI.Adapters;

public interface INavigationViewAdapter
{
    bool CanGoBack { get; }

    public object? Content { get; }

    public void GoBack(bool animated);

    public int BackStackDepth { get; }
    IObservable<INavigationEventArgs> Navigated { get; }
    IObservable<Unit> BackRequested { get; }
    bool IsBackButtonVisible { get; set; }

    public void Pop();

    public void Navigate(Type viewType, bool animate);

    public void ClearStack();

    Window Window { get; }
}
