using System;
using System.Reactive.Concurrency;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using Sextant;
using Splat;

namespace Community.Sextant.WinUI.Splat;

/// <summary>
/// Extension methods associated with the IMutableDependencyResolver interface.
/// </summary>
public static class DependencyResolverMixins
{
    /// <summary>
    /// Gets the navigation view key.
    /// </summary>
    public const string NavigationView = "NavigationView";

    /// <summary>
    /// Initializes the navigation view..
    /// </summary>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <param name="mainThreadScheduler">The main scheduler.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <returns>The dependencyResolver.</returns>
    public static IMutableDependencyResolver RegisterNavigationView(
        this IMutableDependencyResolver dependencyResolver,
        IScheduler? mainThreadScheduler = null,
        IScheduler? backgroundScheduler = null
    )
    {
        dependencyResolver.RegisterLazySingleton(
            () =>
                new NavigationService(
                    mainThreadScheduler ?? RxApp.MainThreadScheduler,
                    backgroundScheduler ?? RxApp.TaskpoolScheduler,
                    Locator.Current.GetService<IViewTypeLocator>()!,
                    Locator.Current.GetService<IDialogManager>()!
                ),
            typeof(IView),
            NavigationView
        );
        return dependencyResolver;
    }

    /// <summary>
    /// Gets the navigation view.
    /// </summary>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <returns>The navigation view.</returns>
    public static INavigationService? GetNavigationView(
        this IReadonlyDependencyResolver dependencyResolver
    )
    {
        return (INavigationService?)dependencyResolver.GetService<IView>(NavigationView);
    }

    /// <summary>
    /// Initializes WinUI-specific view locator.
    /// </summary>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <returns>The dependencyResolver.</returns>
    public static IMutableDependencyResolver RegisterWinUIViewLocator(
        this IMutableDependencyResolver dependencyResolver
    )
    {
        if (dependencyResolver is null)
        {
            throw new ArgumentNullException(nameof(dependencyResolver));
        }

        var vtl = new ViewTypeLocator(ViewLocator.Current);

        dependencyResolver.RegisterConstant(vtl, typeof(ViewTypeLocator));
        dependencyResolver.RegisterConstant(vtl, typeof(IViewTypeLocator));

        return dependencyResolver;
    }

    /// <summary>
    /// Register view for viewmodel, but only return view type for WinUI frame.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The viewmodel type.</typeparam>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <param name="contract">The contract.</param>
    /// <returns>
    /// The dependencyResolver.
    /// </returns>
    public static IMutableDependencyResolver RegisterViewWinUI<TView, TViewModel>(
        this IMutableDependencyResolver dependencyResolver,
        string? contract = null
    )
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IViewModel
    {
        if (dependencyResolver is null)
        {
            throw new ArgumentNullException(nameof(dependencyResolver));
        }

        var uwpViewTypeResolver = Locator.Current.GetService<ViewTypeLocator>();
        if (uwpViewTypeResolver is null)
        {
            throw new InvalidOperationException("WinUI view type resolver not registered.");
        }

        uwpViewTypeResolver.Register<TView, TViewModel>();
        dependencyResolver.Register(
            () => Locator.Current.GetService<TView>()!,
            typeof(IViewFor<TViewModel>),
            contract
        );
        return dependencyResolver;
    }

    /// <summary>
    /// Registers the <seealso cref="IViewStackService"/> with the singleton of <seealso cref="IParameterViewStackService"/>.
    /// </summary>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <returns>The dependencyResolver.</returns>
    public static IMutableDependencyResolver RegisterViewStackServiceFromParameterService(
        this IMutableDependencyResolver dependencyResolver
    )
    {
        if (dependencyResolver is null)
        {
            throw new ArgumentNullException(nameof(dependencyResolver));
        }

        dependencyResolver.RegisterLazySingleton<IViewStackService>(() =>
            Locator.Current.GetService<IParameterViewStackService>());

        return dependencyResolver;
    }
}
