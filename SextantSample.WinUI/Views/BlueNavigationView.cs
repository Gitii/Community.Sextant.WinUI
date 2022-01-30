using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SextantSample.WinUI;

public class BlueNavigationView : Frame
{
    public BlueNavigationView()
    {
        Background = new SolidColorBrush(Colors.Blue);
        Foreground = new SolidColorBrush(Colors.White);
    }
}
