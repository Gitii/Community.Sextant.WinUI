using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using SextantSample.WinUI.ViewModels;

namespace SextantSample.WinUI.Views;

public sealed partial class RootView : UserControl, IViewFor<RootViewModel>
{
    public RootView()
    {
        this.InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                this.Bind(ViewModel, (vm) => vm.ActiveRoute, (x) => x.NavigationView.SelectedItem);
                this.OneWayBind(
                    ViewModel,
                    (vm) => vm.CounterRoutes,
                    (v) => v.NavigationView.MenuItemsSource
                );
            }
        );
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (RootViewModel?)value;
    }

    public RootViewModel? ViewModel { get; set; }

    public Frame GetContent()
    {
        return Content;
    }

    public NavigationView GetNavigationView()
    {
        return NavigationView;
    }
}
