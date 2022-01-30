using System;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using Sextant;

namespace Community.Sextant.WinUI;

/// <summary>
/// Special resolvers that also spits out view types:
/// A view model type is resolved to a view type.
/// Activation of the view type is done by the WinUI <seealso cref="Frame"/>.
/// It extends the default <seealso cref="IViewLocator"/> and delegates calls to
/// the registered service at runtime.
/// </summary>
public interface IViewTypeLocator : IViewLocator
{
    /// <summary>
    /// Register view type with viewmodel type.
    /// </summary>
    /// <typeparam name="TView">View type.</typeparam>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <param name="contract">The contract.</param>
    void Register<TView, TViewModel>(string? contract = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IViewModel;

    /// <summary>
    /// Method to get view type for view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="contract">The contract.</param>
    /// <returns>The view type again.</returns>
    Type? ResolveViewType<TViewModel>(string? contract = null) where TViewModel : class;

    /// <summary>
    /// Method to get view type for view model type.
    /// </summary>
    /// <param name="viewModelType">The view model Type.</param>
    /// <param name="contract">The contract.</param>
    /// <returns>The view type again.</returns>
    Type? ResolveViewType(Type viewModelType, string? contract = null);
}
