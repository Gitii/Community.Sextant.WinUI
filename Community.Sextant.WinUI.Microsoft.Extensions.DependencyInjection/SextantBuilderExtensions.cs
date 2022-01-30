using System;
using Microsoft.Extensions.DependencyInjection;

namespace Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A collection of helper methods that help to initialize sextant.
/// </summary>
public static class SextantBuilderExtensions
{
    /// <summary>
    /// Returns a builder that is used to configure sextant services, views and view models.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <param name="builder">A callback that accepts the builder.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection UseSextant(
        this IServiceCollection collection,
        Action<ISextantBuilder> builder
    )
    {
        SextantBuilder builderInstance = new SextantBuilder(collection);
        builder(builderInstance);

        if (!builderInstance.IsReady)
        {
            throw new Exception(
                "Sextant hasn't been configured properly. Did you call builder.ConfigureDefaults()?"
            );
        }

        return collection;
    }
}
