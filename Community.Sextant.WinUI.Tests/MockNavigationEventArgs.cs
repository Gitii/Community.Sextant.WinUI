using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Sextant.WinUI.Adapters;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace Community.Sextant.WinUI.Tests;

class MockNavigationEventArgs : INavigationEventArgs
{
    public object Content { get; set; } = null!;
    public NavigationMode NavigationMode { get; set; }
    public NavigationTransitionInfo NavigationTransitionInfo { get; set; } = null!;
    public object Parameter { get; set; } = null!;
    public Type SourcePageType { get; set; } = null!;
    public Uri Uri { get; set; } = null!;
}
