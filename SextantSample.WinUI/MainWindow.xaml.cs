using Microsoft.UI.Xaml;
using System;
using Community.Sextant.WinUI;
using Community.Sextant.WinUI.Adapters;
using Community.Sextant.WinUI.Splat;
using Microsoft.UI.Xaml.Controls;
using Splat;
using Sextant;
using SextantSample.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SextantSample.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        var dm = Locator.Current.GetService<IDialogManager>()!;
        dm.SetWindow(this);

        var navigationService = Locator.Current.GetNavigationView()!;

        var frame = new Frame();

        navigationService.SetAdapter(new FrameNavigationViewAdapter(frame, this));

        Content = frame;

        Locator.Current.GetService<IParameterViewStackService>()!
            .PushPage(new HomeViewModel(), null, true, false)
            .Subscribe();
    }
}
