using System;
using System.Collections.Immutable;
using System.Linq;
using Community.Sextant.WinUI;
using ReactiveUI;
using Sextant;

namespace SextantSample.WinUI.ViewModels;

public class RootViewModel : ReactiveObject, IViewModel
{
    private readonly IParameterViewStackService _stackService;
    CounterRoute? _activeRoute;
    ImmutableList<CounterRoute> _counterRoutes;

    public RootViewModel(
        IParameterViewStackService stackService,
        INavigationService _navigationService
    )
    {
        _stackService = stackService;

        _counterRoutes = ImmutableList<CounterRoute>.Empty;
        CounterRoutes = ImmutableList<CounterRoute>.Empty.AddRange(
            new[]
            {
                new CounterRoute("0", 0), new CounterRoute("1", 1), new CounterRoute("2", 2),
                new CounterRoute("3", 3), new CounterRoute("5", 5), new CounterRoute("8", 8),
                new CounterRoute("13", 13), new CounterRoute("21", 21)
            }
        );

        this.WhenAnyValue((vm) => vm.ActiveRoute)
            .Subscribe(
                (ar) =>
                {
                    if (ar is not null)
                    {
                        if (ar.ActivationType == ActivationType.Pushed)
                        {
                            ar.ActivationType = ActivationType.Unknown;
                        }
                        else
                        {
                            _stackService
                                .PushPage<CounterViewModel>(
                                    new NavigationParameter() { { "Counter", ar.Counter } }
                                )
                                .Subscribe();
                        }
                    }
                }
            );

        stackService.PageStack.Subscribe(
            (stack) =>
            {
                var pushedRoute = stack.Cast<CounterViewModel>().LastOrDefault();
                if (
                    ActiveRoute?.Counter != pushedRoute?.Counter
                    && pushedRoute is not null
                )
                {
                    var availableRoute = CounterRoutes.FirstOrDefault(
                        (cr) => cr.Counter == pushedRoute.Counter
                    );

                    if (availableRoute != null)
                    {
                        availableRoute.ActivationType = ActivationType.Pushed;
                    }

                    ActiveRoute =
                        availableRoute;
                }
            }
        );
    }

    public ImmutableList<CounterRoute> CounterRoutes
    {
        get { return _counterRoutes; }
        set { this.RaiseAndSetIfChanged(ref _counterRoutes, value); }
    }

    public CounterRoute? ActiveRoute
    {
        get { return _activeRoute; }
        set { this.RaiseAndSetIfChanged(ref _activeRoute, value); }
    }

    public void Init()
    {
        ActiveRoute = CounterRoutes.FirstOrDefault();
    }

    public string Id { get; } = nameof(RootViewModel);
}
