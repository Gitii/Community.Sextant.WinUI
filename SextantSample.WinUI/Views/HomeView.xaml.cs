using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.UI.Xaml;
using ReactiveUI;
using SextantSample.ViewModels;
using Splat;

namespace SextantSample.WinUI.Views;

public partial class HomeView
{
    private readonly Window _window;

    public HomeView()
    {
        _window = Locator.Current.GetService<Window>()!;
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.BindCommand(ViewModel, x => x.OpenModal, x => x.FirstModalButton)
                    .DisposeWith<IReactiveBinding<HomeView, ReactiveCommand<Unit, Unit>>>(
                        disposables
                    );
                this.BindCommand(ViewModel, x => x.PushPage, x => x.PushPage);
                this.BindCommand(ViewModel, x => x.PushGenericPage, x => x.PushGenericPage)
                    .DisposeWith(disposables);
            }
        );

        Interactions.ErrorMessage.RegisterHandler(
            async x =>
            {
                await Alerts
                    .DisplayAlertAsync(_window, "Error", x.Input.Message, "Done")
                    .ConfigureAwait(false);
                x.SetOutput(true);
            }
        );
    }
}
