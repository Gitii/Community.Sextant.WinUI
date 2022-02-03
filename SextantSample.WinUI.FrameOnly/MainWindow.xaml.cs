using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using Community.Sextant.WinUI;
using Community.Sextant.WinUI.Adapters;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Sextant;
using SextantSample.WinUI.ViewModels;

namespace SextantSample.WinUI.FrameOnly;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;
    private readonly IParameterViewStackService _viewStackService;

    internal MainWindow()
    {
        _navigationService = null!;
        _viewStackService = null!;
    }

    public MainWindow(
        INavigationService navigationService,
        IParameterViewStackService viewStackService
    )
    {
        _navigationService = navigationService;
        _viewStackService = viewStackService;
        this.InitializeComponent();

        var appWindow = GetAppWindowForCurrentWindow();
        appWindow.Title = "Frame only";
        appWindow.Resize(new SizeInt32(400, 400));

        // Tell navigation service which <Frame /> to use
        // A reference to the containing <Window /> is also needed for Popups.
        _navigationService.SetAdapter(new FrameNavigationViewAdapter(Content, this));

        // Now navigate to the first page
        _viewStackService
            .PushPage<CounterViewModel>(new NavigationParameter() { { "Counter", 0 } })
            .Subscribe();
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }
}
