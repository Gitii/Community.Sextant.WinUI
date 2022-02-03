using Microsoft.UI.Xaml;
using System;
using Community.Sextant.WinUI;
using Community.Sextant.WinUI.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using SextantSample.WinUI.ViewModels;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using CounterView = SextantSample.WinUI.Views.CounterView;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SextantSample.WinUI.FrameOnly;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();

        Init();
    }

    public IServiceProvider? Container { get; private set; }

    void Init()
    {
        RxApp.DefaultExceptionHandler = new SextantDefaultExceptionHandler();
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(
                services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    // Configure our local services and access the host configuration
                    ConfigureServices(services);

                    // Configure Sextant, Views and ViewModels
                    services.UseSextant(
                        builder =>
                        {
                            builder.ConfigureDefaults();
                            builder.ConfigureViews(
                                viewBuilder =>
                                {
                                    viewBuilder.RegisterViewAndViewModel<
                                        CounterView,
                                        CounterViewModel
                                    >();
                                }
                            );
                        }
                    );
                }
            )
            .UseEnvironment(Environments.Development)
            .Build();

        // Since MS DI container is a different type,
        // we need to re-register the built container with Splat again
        Container = host.Services;
        Container.UseMicrosoftDependencyResolver();
    }

    void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = Container!.GetRequiredService<MainWindow>();
        m_window.Activate();
    }

    private Window? m_window;
}
