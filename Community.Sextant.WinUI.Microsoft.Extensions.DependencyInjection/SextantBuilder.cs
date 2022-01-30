using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReactiveUI;
using Sextant;

namespace Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;

class SextantBuilder : ISextantBuilder
{
    private readonly IServiceCollection _collection;
    private bool _isReady;
    private IViewTypeLocator? _viewTypeLocator;

    public SextantBuilder(IServiceCollection collection)
    {
        _collection = collection;
    }

    public bool IsReady => _isReady;

    public ISextantBuilder UseViewTypeLocator(IViewTypeLocator customVtl)
    {
        _viewTypeLocator = customVtl;

        return this;
    }

    public ISextantBuilder ConfigureDefaults()
    {
        _collection.TryAddSingleton<IDialogManager, DialogManager>();
        _collection.TryAddSingleton<IViewModelFactory, DefaultViewModelFactory>();

        _viewTypeLocator ??= new ViewTypeLocator(
            ViewLocator.Current ?? throw new Exception("Default ViewLocator is not registered.")
        );

        _collection.TryAddSingleton<IViewTypeLocator>(_viewTypeLocator);

        _collection.TryAddSingleton<IView>(
            (services) =>
            (
                new NavigationService(
                    RxApp.MainThreadScheduler,
                    RxApp.TaskpoolScheduler,
                    services.GetRequiredService<IViewTypeLocator>(),
                    services.GetRequiredService<IDialogManager>()
                )
            )
        );

        _collection.TryAddSingleton<ParameterViewStackService, ParameterViewStackService>();
        _collection.TryAddSingleton<IParameterViewStackService>(
            (services) => services.GetRequiredService<ParameterViewStackService>()
        );
        _collection.TryAddSingleton<IViewStackService>(
            (services) => services.GetRequiredService<ParameterViewStackService>()
        );

        _isReady = true;

        return this;
    }

    public ISextantBuilder ConfigureViews(Action<INavigationViewBuilder> builder)
    {
        builder(
            new NavigationViewBuilder(
                _collection,
                _viewTypeLocator ?? throw new Exception("RegisterDefaults must be called first.")
            )
        );

        return this;
    }
}
