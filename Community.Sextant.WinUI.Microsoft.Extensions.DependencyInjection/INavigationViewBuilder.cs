using ReactiveUI;
using Sextant;

namespace Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An abstract builder that is used to configure views and view models.
/// </summary>
public interface INavigationViewBuilder
{
    /// <summary>
    /// Registers a view and view model relationship.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <returns>The builder.</returns>
    public INavigationViewBuilder RegisterViewAndViewModel<TView, TViewModel>()
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IViewModel;
}
