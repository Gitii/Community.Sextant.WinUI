using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Sextant;

namespace Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;

class NavigationViewBuilder : INavigationViewBuilder
{
    private readonly IServiceCollection _collection;
    private readonly IViewTypeLocator _typeLocator;

    public NavigationViewBuilder(IServiceCollection collection, IViewTypeLocator typeLocator)
    {
        _collection = collection;
        _typeLocator = typeLocator;
    }

    public INavigationViewBuilder RegisterViewAndViewModel<TView, TViewModel>()
        where TView : class, IViewFor<TViewModel>
        where TViewModel : class, IViewModel
    {
        _collection.AddTransient<IViewFor<TViewModel>, TView>();
        _typeLocator.Register<TView, TViewModel>();

        return this;
    }
}
