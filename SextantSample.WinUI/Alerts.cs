﻿using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Community.Sextant.WinUI;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace SextantSample.WinUI;

internal static class Alerts
{
    public static Task DisplayAlertAsync(
        Window window,
        string title,
        string content,
        string defaultButtonContent,
        string cancelButtonContent = "Cancel"
    )
    {
        // Create the message dialog and set its content
        var messageDialog = new MessageDialog(content, title);

        // Associate the HWND with the folder picker
        InitializeWithWindow.Initialize(messageDialog, window.GetHandle());

        var handler = new UICommandInvokedHandler((_) => { });

        // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
        messageDialog.Commands.Add(new UICommand(defaultButtonContent, handler));
        messageDialog.Commands.Add(new UICommand(cancelButtonContent, handler));

        // Set the command that will be invoked by default
        messageDialog.DefaultCommandIndex = 0;

        // Set the command to be invoked when escape is pressed
        messageDialog.CancelCommandIndex = 1;

        // Show the message dialog
        return messageDialog.ShowAsync().AsTask();
    }
}
