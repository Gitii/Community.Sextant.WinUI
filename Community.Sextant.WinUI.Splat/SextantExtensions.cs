using System;
using Sextant;

namespace Community.Sextant.WinUI.Splat;

/// <summary>
/// Extension methods interact with <see cref="Sextant"/>.
/// </summary>
public static class SextantExtensions
{
    /// <summary>
    /// Initializes the sextant.
    /// </summary>
    /// <param name="sextant">The sextant.</param>
    public static void Initialize(this global::Sextant.Sextant sextant)
    {
        if (sextant is null)
        {
            throw new ArgumentNullException(nameof(sextant));
        }

        sextant.MutableLocator
            .RegisterWinUIViewLocator()
            .RegisterNavigationView()
            .RegisterViewStackService()
            .RegisterParameterViewStackService()
            .RegisterViewModelFactory(() => new DefaultViewModelFactory());
    }
}
