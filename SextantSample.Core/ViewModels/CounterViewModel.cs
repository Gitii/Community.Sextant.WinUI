using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Sextant;

namespace SextantSample.WinUI.ViewModels;

public class CounterViewModel : ReactiveObject, INavigable
{
    private readonly IParameterViewStackService _viewStackService;
    int _counter;

    public CounterViewModel(IParameterViewStackService viewStackService)
    {
        _viewStackService = viewStackService;
        _counter = 0;

        PushPage = ReactiveCommand.Create(
            () =>
            {
                _viewStackService
                    .PushPage<CounterViewModel>(
                        new NavigationParameter() { { "Counter", Counter + 1 } }
                    )
                    .Subscribe();
            }
        );
    }

    public int Counter
    {
        get { return _counter; }
        set { this.RaiseAndSetIfChanged(ref _counter, value); }
    }

    public ReactiveCommand<Unit, Unit> PushPage { get; set; }

    public string Id { get; } = nameof(CounterViewModel);

    public IObservable<Unit> WhenNavigatedTo(INavigationParameter parameter)
    {
        return Observable.Return(Unit.Default);
    }

    public IObservable<Unit> WhenNavigatedFrom(INavigationParameter parameter)
    {
        return Observable.Return(Unit.Default);
    }

    public IObservable<Unit> WhenNavigatingTo(INavigationParameter parameter)
    {
        Counter = parameter.GetValue<int>("Counter");

        return Observable.Return(Unit.Default);
    }
}
