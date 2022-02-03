using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Community.Sextant.WinUI.Adapters;
using Microsoft.UI.Xaml.Navigation;
using ReactiveUI;
using Sextant;
using Splat;

namespace Community.Sextant.WinUI;

public class NavigationService : IEnableLogger, INavigationService, IDisposable
{
    private readonly IDialogManager _dialogManager;
    private readonly IFullLogger _logger;
    private readonly IViewTypeLocator _viewLocator;
    private Subject<Unit> _backRequestedSubject;

    private CompositeDisposable? _mainDisposable;

    private INavigationViewAdapter _navigationViewAdapter;
    private IDisposable? _navigationViewAdapterDisposeable;

    private Subject<INavigationEventArgs> _pagePoppedSubject;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="mainScheduler">The main scheduler to scheduler UI tasks on.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="viewLocator">The view locator which will find views associated with view models.</param>
    /// <param name="dialogManager">The dialog manager for the current window.</param>
    /// <param name="logger">(Optional) An instance of a logger that's used for logging.</param>
    public NavigationService(
        IScheduler mainScheduler,
        IScheduler backgroundScheduler,
        IViewTypeLocator viewLocator,
        IDialogManager dialogManager,
        IFullLogger? logger = null
    )
    {
        _logger = logger ?? this.Log();
        _mainDisposable = new CompositeDisposable();

        BackgroundScheduler = backgroundScheduler;
        MainThreadScheduler = mainScheduler;
        _viewLocator = viewLocator;
        _dialogManager = dialogManager;

        MirroredPageStack = new Stack<IViewModel?>();
        MirroredModalStack = new Stack<IViewModel?>();

        _navigationViewAdapter = new NullNavigationViewAdapter();
        _navigationViewAdapterDisposeable = null;
        _pagePoppedSubject = new Subject<INavigationEventArgs>();
        _backRequestedSubject = new Subject<Unit>();

        SetAdapter(_navigationViewAdapter);

        PagePopped = BuildPagePoppedObservable();

        BackRequested = BuildBackRequestedObservable();

        BackRequested.Subscribe();
    }

    internal IDialog? ContentDialog { get; set; }

    internal IViewModel? LastPoppedViewModel { get; set; }

    internal Stack<IViewModel?> MirroredPageStack { get; }
    internal Stack<IViewModel?> MirroredModalStack { get; }

    public void Dispose()
    {
        _mainDisposable?.Dispose();
        _mainDisposable = null;
    }

    /// <summary>
    /// Gets a value indicating whether a modal is visible.
    /// </summary>
    public bool ModalVisible
    {
        get { return _dialogManager.AreModalDialogsActive; }
    }

    /// <summary>
    /// Gets the background scheduler.
    /// </summary>
    public IScheduler BackgroundScheduler { get; }

    /// <inheritdoc />
    public IScheduler MainThreadScheduler { get; }

    /// <inheritdoc />
    public IObservable<IViewModel?> PagePopped { get; }

    /// <summary>
    /// Gets combined back requested observable from system, back button, and xbox controller sources.
    /// </summary>
    public IObservable<Unit> BackRequested { get; }

    /// <inheritdoc />
    public IObservable<Unit> PopModal()
    {
        if (ContentDialog is null)
        {
            return Observable.Return(Unit.Default).ObserveOn(MainThreadScheduler);
        }

        return Observable
            .FromAsync(CloseAllOpenDialogsAsync, MainThreadScheduler)
            .ObserveOn(MainThreadScheduler)
            .Select(
                (p) =>
                {
                    MirroredModalStack.TryPop(out _);

                    if (!MirroredModalStack.TryPeek(out var modalViewModel))
                    {
                        return Unit.Default;
                    }

                    if (modalViewModel is null)
                    {
                        return Unit.Default;
                    }

                    var view = ResolveViewInstanceFor(modalViewModel, null);
                    view.ViewModel = modalViewModel;

                    ShowContentWithDialog(view);

                    return Unit.Default;
                }
            );
    }

    /// <inheritdoc />
    public IObservable<Unit> PopPage(bool animate)
    {
        MirroredPageStack.Pop();

        var content = _navigationViewAdapter.Content;
        if (content is not IViewFor view)
        {
            var contentTypeName = content?.GetType().ToString() ?? "Unknown Type";
            _logger.Debug(
                $"The view ({contentTypeName}) does not implement IViewFor<>.  Cannot get ViewModel."
            );
            return Observable.Return(Unit.Default).ObserveOn(MainThreadScheduler);
        }

        LastPoppedViewModel = view.ViewModel as IViewModel;

        _navigationViewAdapter.GoBack(animate);

        return Observable.Return(Unit.Default).ObserveOn(MainThreadScheduler);
    }

    /// <inheritdoc />
    public IObservable<Unit> PopToRootPage(bool animate)
    {
        while (_navigationViewAdapter.BackStackDepth > 1)
        {
            _navigationViewAdapter.Pop();
        }

        while (MirroredPageStack.Count > 1)
        {
            MirroredPageStack.Pop();
        }

        var content = _navigationViewAdapter.Content;
        if (content is not IViewFor view)
        {
            var contentTypeName = content?.GetType().ToString() ?? "Unknown Type";
            _logger.Debug(
                $"The view ({contentTypeName}) does not implement IViewFor<>.  Cannot get ViewModel."
            );
            return Observable.Return(Unit.Default).ObserveOn(MainThreadScheduler);
        }

        LastPoppedViewModel = view.ViewModel as IViewModel;

        _navigationViewAdapter.GoBack(animate);

        return Observable.Return(Unit.Default).ObserveOn(MainThreadScheduler);
    }

    /// <inheritdoc />
    public IObservable<Unit> PushModal(
        IViewModel modalViewModel,
        string? contract,
        bool withNavigationPage = true
    ) =>
        Observable
            .FromAsync(CloseAllOpenDialogsAsync, MainThreadScheduler)
            .ObserveOn(MainThreadScheduler)
            .Select(
                (_) =>
                {
                    MirroredModalStack.Push(modalViewModel);

                    var view = ResolveViewInstanceFor(modalViewModel, contract);
                    view.ViewModel = modalViewModel;

                    ShowContentWithDialog(view);

                    return Unit.Default;
                }
            );

    /// <inheritdoc />
    public IObservable<Unit> PushPage(
        IViewModel viewModel,
        string? contract,
        bool resetStack,
        bool animate
    ) =>
        Observable
            .Start(() => ResolveViewTypeFor(viewModel, contract), MainThreadScheduler)
            .ObserveOn(MainThreadScheduler)
            .SelectMany(
                pageType =>
                {
                    if (resetStack)
                    {
                        MirroredPageStack.Clear();
                        MirroredPageStack.Push(viewModel);

                        _navigationViewAdapter.Navigate(pageType, animate);

                        // NOTE: this assumes that Navigate already updated
                        // the active content with the new page
                        var content = _navigationViewAdapter.Content;
                        if (content is IViewFor viewForReset)
                        {
                            viewForReset.ViewModel = viewModel;
                        }
                        else
                        {
                            var contentTypeName = content?.GetType().ToString() ?? "Unknown Type";
                            _logger.Debug(
                                $"The view ({contentTypeName}) does not implement IViewFor<>.  Cannot set ViewModel of type, {viewModel.GetType()}, on view."
                            );
                        }

                        _navigationViewAdapter.ClearStack();

                        return Observable.Return(Unit.Default);
                    }

                    MirroredPageStack.Push(viewModel);

                    _navigationViewAdapter.Navigate(pageType, animate);
                    if (_navigationViewAdapter.Content is IViewFor viewFor)
                    {
                        viewFor.ViewModel = viewModel;
                    }
                    else
                    {
                        var contentTypeName =
                            _navigationViewAdapter.Content?.GetType().ToString() ?? "Unknown Type";
                        _logger.Debug(
                            $"The view ({contentTypeName}) does not implement IViewFor<>.  Cannot set ViewModel of type, {viewModel.GetType()}, on view."
                        );
                    }

                    _navigationViewAdapter.IsBackButtonVisible = _navigationViewAdapter.CanGoBack;

                    return Observable.Return(Unit.Default);
                }
            );

    public void SetAdapter(INavigationViewAdapter adapter)
    {
        if (_navigationViewAdapterDisposeable != null)
        {
            _navigationViewAdapterDisposeable.Dispose();
            _navigationViewAdapterDisposeable = null;
        }

        _navigationViewAdapterDisposeable = new CompositeDisposable(
            adapter.Navigated.Subscribe(_pagePoppedSubject),
            adapter.BackRequested.Subscribe(_backRequestedSubject)
        );

        _navigationViewAdapter = adapter;
    }

    private IConnectableObservable<Unit> BuildBackRequestedObservable()
    {
        var observable = _backRequestedSubject
            .Do(
                _ =>
                {
                    if (_navigationViewAdapter.CanGoBack)
                    {
                        PopPage(true);
                    }
                }
            )
            .Select(_ => Unit.Default)
            .Publish();

        _mainDisposable?.Add(observable.Connect());
        return observable;
    }

    private async Task<Unit> CloseAllOpenDialogsAsync()
    {
        if (ContentDialog is not null && ModalVisible)
        {
            ContentDialog.Hide();
            ContentDialog = null;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(true);

        while (ModalVisible)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(true);
        }

        return Unit.Default;
    }

    private void ShowContentWithDialog(object content)
    {
        ContentDialog = _dialogManager.Create(content);

        ContentDialog.Show();
    }

    private IObservable<IViewModel?> BuildPagePoppedObservable()
    {
        var observable = _pagePoppedSubject
            .Where(ep => ep.NavigationMode == NavigationMode.Back)
            .Do(
                (ep) =>
                    _navigationViewAdapter.IsBackButtonVisible = _navigationViewAdapter.CanGoBack
            )
            .Select(
                ep =>
                {
                    if (ep.Content is not IViewFor view)
                    {
                        _logger.Debug(
                            $"The view ({ep.Content?.GetType()}) does not implement IViewFor<>.  Cannot set ViewModel from a back navigation."
                        );
                    }
                    else
                    {
                        view.ViewModel = MirroredPageStack.Peek();
                    }

                    // Since view stack doesn't contain instances (only types), we have to store the latest viewmodel and return it on a back nav.
                    // ep.Content contains an instance of the new view, but it may have just been created and its ViewModel property will be null.
                    // But we want the view that was just removed.  We need to send the old view's viewmodel to IViewStackService so that the ViewModel can be removed from the stack.
                    return LastPoppedViewModel;
                }
            )
            .Where(x => x is not null)
            .Publish();

        _mainDisposable?.Add(observable.Connect());

        return observable;
    }

    private Type ResolveViewTypeFor(object viewModel, string? contract)
    {
        var viewType = _viewLocator.ResolveViewType(viewModel.GetType(), contract);

        if (viewType is null)
        {
            throw new InvalidOperationException(
                $"No view type could be located for type '{viewModel.GetType().FullName}', contract '{contract}'. Be sure Splat has an appropriate registration."
            );
        }

        return viewType;
    }

    private IViewFor ResolveViewInstanceFor(object viewModel, string? contract)
    {
        var view = _viewLocator.ResolveView(viewModel, contract);

        if (view is null)
        {
            throw new InvalidOperationException(
                $"No view could be located for type '{viewModel.GetType().FullName}', contract '{contract}'. Be sure Splat has an appropriate registration."
            );
        }

        return view;
    }
}
