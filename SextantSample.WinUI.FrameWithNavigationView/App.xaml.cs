using System.Reflection;
using Community.Sextant.WinUI;
using Community.Sextant.WinUI.Splat;
using Microsoft.UI.Xaml;
using ReactiveUI;
using Sextant;
using SextantSample.WinUI.ViewModels;
using SextantSample.WinUI.Views;
using Splat;
using CounterView = SextantSample.WinUI.Views.CounterView;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SextantSample.WinUI.FrameWithNavigationView;

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

    void Init()
    {
        RxApp.DefaultExceptionHandler = new SextantDefaultExceptionHandler();
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable
            .RegisterWinUIViewLocator()
            .RegisterParameterViewStackService()
            .RegisterViewStackServiceFromParameterService()
            .RegisterNavigationView()
            .RegisterViewWinUI(
                () => new CounterView(),
                () =>
                    new CounterViewModel(Locator.Current.GetService<IParameterViewStackService>()!)
            )
            .RegisterViewWinUI(
                () => new RootView(),
                () =>
                    new RootViewModel(
                        Locator.Current.GetService<IParameterViewStackService>()!,
                        Locator.Current.GetService<INavigationService>()!
                    )
            )
            .RegisterConstantAnd<IDialogManager>(new DialogManager())
            .RegisterLazySingletonAnd(
                () =>
                    new MainWindow(
                        Locator.Current.GetService<INavigationService>()!,
                        Locator.Current.GetService<RootViewModel>()!
                    )
            );
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = Locator.Current.GetService<MainWindow>()!;
        m_window.Activate();
    }

    private Window? m_window;
}
