﻿using Microsoft.UI.Xaml;
using ReactiveUI;
using SextantSample.ViewModels;
using Splat;

namespace SextantSample.WinUI.Views;

public partial class SecondModalView
{
    private readonly Window _window;

    public SecondModalView()
    {
        _window = Locator.Current.GetService<Window>()!;
        InitializeComponent();
        this.BindCommand(ViewModel, x => x.PushPage, x => x.PushPage);
        this.BindCommand(ViewModel, x => x.PopModal, x => x.PopModal);

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
