using System;
using System.Collections.Generic;
using ReactiveUI;
using Sextant;

namespace Community.Sextant.WinUI;

/// <inheritdoc />
public class ViewTypeLocator : IViewTypeLocator
{
    private readonly IViewLocator _viewLocator;

    private readonly Dictionary<(string VmTypeName, string? Contract), Type> _typeDictionary =
        new();

    public ViewTypeLocator(IViewLocator viewLocator)
    {
        _viewLocator = viewLocator;
    }

    /// <inheritdoc />
    public void Register<TView, TViewModel>(string? contract = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IViewModel
    {
        if (_typeDictionary.ContainsKey((typeof(TViewModel).AssemblyQualifiedName!, contract)))
        {
            throw new Exception("Type already registered.");
        }

        _typeDictionary.Add((typeof(TViewModel).AssemblyQualifiedName!, contract), typeof(TView));
    }

    /// <inheritdoc />
    public Type? ResolveViewType<TViewModel>(string? contract = null) where TViewModel : class
    {
        return ResolveViewType(typeof(TViewModel), contract);
    }

    /// <inheritdoc />
    public Type? ResolveViewType(Type viewModelType, string? contract = null)
    {
        if (viewModelType is null)
        {
            throw new ArgumentNullException(nameof(viewModelType));
        }

        _typeDictionary.TryGetValue(
            (viewModelType.AssemblyQualifiedName!, contract),
            out var value
        );

        return value;
    }

    /// <inheritdoc />
    public IViewFor? ResolveView<T>(T viewModel, string? contract = null)
    {
        return _viewLocator.ResolveView(viewModel, contract);
    }
}
