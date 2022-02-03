using System.Reactive.Disposables;
using ReactiveUI;

namespace SextantSample.WinUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CounterView
{
    public CounterView()
    {
        this.InitializeComponent();

        this.WhenActivated(
            (disposable) =>
            {
                this.BindCommand(ViewModel, (vm) => vm.PushPage, (v) => v.Button)
                    .DisposeWith(disposable);

                this.Bind(
                    ViewModel,
                    (vm) => vm.Counter,
                    (v) => v.Label.Text,
                    (int counter) => $"{counter}"
                );
            }
        );
    }
}
