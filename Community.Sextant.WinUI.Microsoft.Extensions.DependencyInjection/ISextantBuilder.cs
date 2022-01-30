using System;

namespace Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An abstract builder that is used to configure sextant services, views and view models.
/// </summary>
public interface ISextantBuilder
{
    /// <summary>
    /// Configures all required services (if not already configured).
    /// Needs to be called.
    /// </summary>
    /// <returns>The sextant builder</returns>
    public ISextantBuilder ConfigureDefaults();

    /// <summary>
    /// Accepts a callback with an abstract builder that is used to configure all views and view models.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The sextant builder.</returns>
    public ISextantBuilder ConfigureViews(Action<INavigationViewBuilder> builder);

    /// <summary>
    /// Tells the builder to use a custom instance of <seealso cref="IViewTypeLocator"/> instead of the default implementation.
    /// Needs to be called before <seealso cref="ConfigureDefaults"/>. Otherwise the default implementation will be used.
    /// </summary>
    /// <param name="customVtl">The custom implementation of <seealso cref="IViewTypeLocator"/>.</param>
    /// <returns>The sextant builder.</returns>
    ISextantBuilder UseViewTypeLocator(IViewTypeLocator customVtl);
}
