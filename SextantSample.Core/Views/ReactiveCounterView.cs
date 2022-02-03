using ReactiveUI;
using SextantSample.WinUI.ViewModels;

namespace SextantSample.WinUI.Views;

/// <summary>
/// Workaround for WinUI 3's missing support for generic base classes in xaml
/// </summary>
public class ReactiveCounterView : ReactivePage<CounterViewModel> { }
