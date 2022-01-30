using ReactiveUI;

namespace SextantSample.WinUI.Views;

public partial class GreenView
{
    public GreenView()
    {
        InitializeComponent();

        this.BindCommand(ViewModel, x => x.OpenModal, x => x.Modal);
    }
}
