using System;
using Windows.Graphics;
using Community.Sextant.WinUI;
using Community.Sextant.WinUI.Adapters;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SextantSample.WinUI.ViewModels;
using SextantSample.WinUI.Views;

namespace SextantSample.WinUI.FrameWithNavigationView;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;
    private RootView? _rootView;

    internal MainWindow()
    {
        _navigationService = null!;
    }

    public MainWindow(INavigationService navigationService, RootViewModel rvm)
    {
        _navigationService = navigationService;
        this.InitializeComponent();

        var appWindow = GetAppWindowForCurrentWindow();
        appWindow.Title = "Frame with NavigationView";
        appWindow.Resize(new SizeInt32(1000, 800));

        _rootView = new RootView() { ViewModel = rvm };
        _rootView.Loaded += RootViewOnLoaded;

        Content = _rootView;
    }

    private void RootViewOnLoaded(object sender, RoutedEventArgs e)
    {
        // Tell navigation service which <Frame /> to use
        // A reference to the containing <Window /> is also needed for Popups.
        _navigationService.SetAdapter(
            new NavigationViewAdapter(_rootView!.GetContent(), this, _rootView!.GetNavigationView())
        );

        _rootView.ViewModel?.Init();
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }
}
