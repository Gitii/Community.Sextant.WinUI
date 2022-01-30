using System;
using System.Reactive;
using System.Reactive.Concurrency;
using Community.Sextant.WinUI.Adapters;
using Sextant;

namespace Community.Sextant.WinUI;

/// <summary>
/// Defines a view that be add to a navigation or modal stack.
/// </summary>
public interface INavigationService : IView
{
    /// <summary>
    /// Gets a value indicating whether a modal (ContentDialog) is visible.
    /// </summary>
    bool ModalVisible { get; }

    /// <summary>
    /// Gets the background scheduler.
    /// </summary>
    IScheduler BackgroundScheduler { get; }

    /// <inheritdoc />
    IScheduler MainThreadScheduler { get; }

    /// <inheritdoc />
    IObservable<IViewModel?> PagePopped { get; }

    /// <summary>
    /// Gets combined back requested observable from system, back button, and xbox controller sources.
    /// </summary>
    IObservable<Unit> BackRequested { get; }

    /// <inheritdoc />
    IObservable<Unit> PopModal();

    /// <inheritdoc />
    IObservable<Unit> PopPage(bool animate);

    /// <inheritdoc />
    IObservable<Unit> PopToRootPage(bool animate);

    /// <inheritdoc />
    IObservable<Unit> PushModal(
        IViewModel modalViewModel,
        string? contract,
        bool withNavigationPage = true
    );

    /// <inheritdoc />
    IObservable<Unit> PushPage(
        IViewModel viewModel,
        string? contract,
        bool resetStack,
        bool animate
    );

    void SetAdapter(INavigationViewAdapter adapter);
}
