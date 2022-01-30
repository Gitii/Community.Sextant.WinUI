using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using ABI.Windows.Foundation;
using Community.Sextant.WinUI.Adapters;
using FakeItEasy;
using FluentAssertions;
using Microsoft.UI.Xaml.Navigation;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ReactiveUI;
using Sextant;
using Splat;

namespace Community.Sextant.WinUI.Tests;

public class NavigationServiceTests
{
    [Test]
    public void Test_Constructor_ShouldInitEverything()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var dm = A.Fake<IDialogManager>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);

        ns.MainThreadScheduler.Should().Be(mainScheduler);
        ns.BackgroundScheduler.Should().Be(bgScheduler);
        ns.PagePopped.Should().NotBeNull();
        ns.BackRequested.Should().NotBeNull();
    }

    [Test]
    public void Test_BuildPagePoppedObservable_ShouldReturnMV()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var navigatedSubject = new Subject<INavigationEventArgs>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Navigated).Returns(navigatedSubject);

        var viewFor = A.Fake<IViewFor>();
        var lastPoppedViewModel = A.Fake<IViewModel>();
        var pageStackMV = A.Fake<IViewModel>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.MirroredPageStack.Push(pageStackMV);
        ns.LastPoppedViewModel = lastPoppedViewModel;

        ns.SetAdapter(adapter);

        var callbackVisited = 0;

        ns.PagePopped.Subscribe(
            (vm) =>
            {
                vm.Should().Be(lastPoppedViewModel);
                callbackVisited++;
            }
        );

        navigatedSubject.OnNext(
            new MockNavigationEventArgs() { NavigationMode = NavigationMode.Back, Content = viewFor }
        );

        callbackVisited.Should().Be(1, "Observable should have fired and called the observer.");

        ns.MirroredPageStack.Count.Should().Be(1);

        A.CallToSet(() => adapter.IsBackButtonVisible).MustHaveHappened();

        A.CallToSet(() => viewFor.ViewModel).To(pageStackMV).MustHaveHappened();
    }

    [Test]
    public void Test_BuildPagePoppedObservable_ShouldReturnMV_ButNoContent()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var navigatedSubject = new Subject<INavigationEventArgs>();
        var logger = A.Fake<IFullLogger>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Navigated).Returns(navigatedSubject);

        var lastPoppedViewModel = A.Fake<IViewModel>();
        var pageStackMV = A.Fake<IViewModel>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm, logger);
        ns.MirroredPageStack.Push(pageStackMV);
        ns.LastPoppedViewModel = lastPoppedViewModel;

        ns.SetAdapter(adapter);

        var callbackVisited = 0;

        ns.PagePopped.Subscribe(
            (vm) =>
            {
                vm.Should().Be(lastPoppedViewModel);
                callbackVisited++;
            }
        );

        navigatedSubject.OnNext(
            new MockNavigationEventArgs() { NavigationMode = NavigationMode.Back }
        );

        callbackVisited.Should().Be(1, "Observable should have fired and called the observer.");

        ns.MirroredPageStack.Count.Should().Be(1);

        A.CallToSet(() => adapter.IsBackButtonVisible).MustHaveHappened();
        A.CallTo(() => logger.Debug(A<string>.Ignored)).MustHaveHappened();
    }

    [Test]
    public void Test_BuildPagePoppedObservable_ShouldNotReturnMVBecauseNotBackwardNavigated()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var navigatedSubject = new Subject<INavigationEventArgs>();
        var navArgs = A.Fake<INavigationEventArgs>();
        var dm = A.Fake<IDialogManager>();

        A.CallTo(() => navArgs.NavigationMode).Returns(NavigationMode.Forward);

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Navigated).Returns(navigatedSubject);
        var pageStackMV = A.Fake<IViewModel>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.MirroredPageStack.Push(pageStackMV);

        ns.PagePopped.Subscribe((vm) => throw new Exception("Should not be called"));

        ns.SetAdapter(adapter);

        navigatedSubject.OnNext(navArgs);

        A.CallTo(() => adapter.IsBackButtonVisible).MustNotHaveHappened();
        A.CallTo(() => navArgs.NavigationMode).MustHaveHappened();
    }

    [Test]
    public void Test_BackRequested_ShouldReturnUnit_ButNoPopPage()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var backRequestedSubject = new Subject<Unit>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.BackRequested).Returns(backRequestedSubject);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);

        var callbackFired = 0;

        ns.BackRequested.Subscribe(
            (vm) =>
            {
                callbackFired++;
            }
        );

        ns.SetAdapter(adapter);

        backRequestedSubject.OnNext(Unit.Default);

        callbackFired.Should().Be(1, "BackRequested shoud have fired.");

        A.CallTo(() => adapter.CanGoBack).MustHaveHappened();
        A.CallTo(() => adapter.Content).MustNotHaveHappened();
    }

    [Test]
    public void Test_BackRequested_ShouldReturnUnit_AndPopPage()
    {
        var mainScheduler = A.Fake<IScheduler>();
        var bgScheduler = A.Fake<IScheduler>();
        var vl = A.Fake<IViewTypeLocator>();
        var backRequestedSubject = new Subject<Unit>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.BackRequested).Returns(backRequestedSubject);
        A.CallTo(() => adapter.CanGoBack).Returns(true);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);

        ns.MirroredPageStack.Push(null);

        var callbackFired = 0;

        ns.SetAdapter(adapter);
        ns.BackRequested.Subscribe(
            (vm) =>
            {
                callbackFired++;
            }
        );

        backRequestedSubject.OnNext(Unit.Default);

        callbackFired.Should().Be(1, "BackRequested shoud have fired.");

        A.CallTo(() => adapter.CanGoBack).MustHaveHappened();
        A.CallTo(() => adapter.Content).MustHaveHappened();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Test_PopPage_ShouldGoBackAsync(bool animated)
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var vl = A.Fake<IViewTypeLocator>();
        var viewFor = A.Fake<IViewFor>();
        var vm = A.Fake<IViewModel>();
        var dm = A.Fake<IDialogManager>();

        A.CallTo(() => viewFor.ViewModel).Returns(vm);

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns(viewFor);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);

        await ns.PopPage(animated).FirstAsync();

        ns.LastPoppedViewModel.Should().Be(vm);

        ns.MirroredPageStack.Should().BeEmpty();

        A.CallTo(() => adapter.GoBack(animated)).MustHaveHappened();
    }

    [Test]
    public async Task Test_PopPage_ShouldNotGoBackAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var vl = A.Fake<IViewTypeLocator>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns("foobar");

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);

        await ns.PopPage(true).FirstAsync();

        ns.LastPoppedViewModel.Should().BeNull();
        ns.MirroredPageStack.Should().BeEmpty();

        A.CallTo(() => adapter.GoBack(A<bool>._)).MustNotHaveHappened();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Test_PopToRootPage_ShouldGoBackAsync(bool animated)
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var vl = A.Fake<IViewTypeLocator>();
        var viewFor = A.Fake<IViewFor>();
        var vm = A.Fake<IViewModel>();
        var dm = A.Fake<IDialogManager>();

        A.CallTo(() => viewFor.ViewModel).Returns(vm);

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns(viewFor);
        A.CallTo(() => adapter.BackStackDepth)
            .Returns(2)
            .Once()
            .Then.Returns(1)
            .Once()
            .Then.Returns(0);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);
        ns.MirroredPageStack.Push(null);

        await ns.PopToRootPage(animated).FirstAsync();

        ns.LastPoppedViewModel.Should().Be(vm);

        ns.MirroredPageStack.Count.Should().Be(1);

        A.CallTo(() => adapter.Pop()).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => adapter.GoBack(animated)).MustHaveHappened();
    }

    [Test]
    public async Task Test_PopToRootPage_ShouldNotGoBackAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var vl = A.Fake<IViewTypeLocator>();
        var dm = A.Fake<IDialogManager>();

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns("foobar");

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);

        await ns.PopToRootPage(true).FirstAsync();

        ns.LastPoppedViewModel.Should().BeNull();
        ns.MirroredPageStack.Should().NotBeEmpty();

        A.CallTo(() => adapter.GoBack(A<bool>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task Test_PopModal_NoDialogVisible_ShouldDoNothingAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var dm = A.Fake<IDialogManager>();
        var vl = A.Fake<IViewTypeLocator>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);

        await ns.PopModal().FirstAsync();

        ns.MirroredModalStack.Should().BeEmpty();

        A.CallTo(() => dm.AreModalDialogsActive).MustNotHaveHappened();
        A.CallTo(() => dm.Create(A<object>._)).MustNotHaveHappened();
    }

    private static readonly object[] TestCases_PopModal_ShouldOnlyPop = new[]
    {
        new object?[] { new IViewModel?[] { null } }, new object?[] { new IViewModel?[] { null, null } }
    };

    [TestCaseSource(nameof(TestCases_PopModal_ShouldOnlyPop))]
    public async Task Test_PopModal_OnlyOnceDialogVisible_ShouldOnlyPopAsync(
        IViewModel?[] pushedVms
    )
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var vl = A.Fake<IViewTypeLocator>();

        var dm = A.Fake<IDialogManager>();
        A.CallTo(() => dm.AreModalDialogsActive)
            .Returns(true)
            .Once()
            .Then.Returns(true)
            .Once()
            .Then.Returns(false);

        var oldDialog = A.Fake<IDialog>();
        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.ContentDialog = oldDialog;

        foreach (var pushedVm in pushedVms)
        {
            ns.MirroredModalStack.Push(pushedVm);
        }

        await ns.PopModal().FirstAsync();

        ns.MirroredModalStack.Should().HaveCount(pushedVms.Length - 1);

        A.CallTo(() => oldDialog.Hide()).MustHaveHappened();
        A.CallTo(() => dm.AreModalDialogsActive).MustHaveHappened();
        A.CallTo(() => dm.Create(A<object>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task Test_PopModal_ShouldPopPopupAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;

        var newDialog = A.Fake<IDialog>();
        var dm = A.Fake<IDialogManager>();
        A.CallTo(() => dm.AreModalDialogsActive).Returns(true).Once().Then.Returns(false);
        A.CallTo(() => dm.Create(A<object>._)).Returns(newDialog);

        var oldVm = A.Fake<IViewModel>();
        var newViewFor = A.Fake<IViewFor>();
        var vl = A.Fake<IViewTypeLocator>();
        A.CallTo(() => vl.ResolveView<object>(oldVm, null)).Returns(newViewFor);

        var adapter = A.Fake<INavigationViewAdapter>();

        var oldDialog = A.Fake<IDialog>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredModalStack.Push(oldVm);
        ns.MirroredModalStack.Push(null);
        ns.ContentDialog = oldDialog;

        await ns.PopModal().FirstAsync();

        ns.MirroredModalStack.Should().HaveCount(1);

        A.CallTo(() => dm.AreModalDialogsActive).MustHaveHappened();
        A.CallTo(() => dm.Create(newViewFor)).MustHaveHappened();
        A.CallTo(() => newDialog.Hide()).MustNotHaveHappened();
        A.CallTo(() => newDialog.Show()).MustHaveHappened();
        A.CallTo(() => oldDialog.Hide()).MustHaveHappened();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Test_PushPage_ShouldResetAndPushAsync(bool animated)
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var viewFor = A.Fake<IViewFor>();
        var vm = A.Fake<IViewModel>();
        var dm = A.Fake<IDialogManager>();

        var viewType = typeof(NavigationServiceTests);
        var vl = A.Fake<IViewTypeLocator>();
        A.CallTo(() => vl.ResolveViewType(vm.GetType(), null)).Returns(viewType);

        A.CallTo(() => viewFor.ViewModel).Returns(vm);

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns(viewFor);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);

        await ns.PushPage(vm, null, true, animated).FirstAsync();

        viewFor.ViewModel.Should().Be(vm);

        ns.MirroredPageStack.Should().HaveCount(1);
        ns.MirroredPageStack.Peek().Should().Be(vm);

        A.CallTo(() => adapter.Navigate(viewType, animated)).MustHaveHappened();
        A.CallTo(() => adapter.ClearStack()).MustHaveHappened();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Test_PushPage_ShouldPushAsync(bool animated)
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var viewFor = A.Fake<IViewFor>();
        var vm = A.Fake<IViewModel>();
        var dm = A.Fake<IDialogManager>();

        var viewType = typeof(NavigationServiceTests);
        var vl = A.Fake<IViewTypeLocator>();
        A.CallTo(() => vl.ResolveViewType(vm.GetType(), null)).Returns(viewType);

        A.CallTo(() => viewFor.ViewModel).Returns(vm);

        var adapter = A.Fake<INavigationViewAdapter>();
        A.CallTo(() => adapter.Content).Returns(viewFor);

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredPageStack.Push(null);

        await ns.PushPage(vm, null, false, animated).FirstAsync();

        viewFor.ViewModel.Should().Be(vm);

        ns.MirroredPageStack.Should().HaveCount(2);
        ns.MirroredPageStack.Peek().Should().Be(vm);

        A.CallTo(() => adapter.Navigate(viewType, animated)).MustHaveHappened();
        A.CallToSet(() => adapter.IsBackButtonVisible).MustHaveHappened();
    }

    [Test]
    public async Task Test_PushModal_ShouldPushAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;

        var newDialog = A.Fake<IDialog>();
        var dm = A.Fake<IDialogManager>();
        A.CallTo(() => dm.AreModalDialogsActive).Returns(true).Once().Then.Returns(false);
        A.CallTo(() => dm.Create(A<object>._)).Returns(newDialog);

        var oldVm = A.Fake<IViewModel>();
        var newVm = A.Fake<IViewModel>();
        var newViewFor = A.Fake<IViewFor>();
        var vl = A.Fake<IViewTypeLocator>();
        A.CallTo(() => vl.ResolveView<object>(newVm, null)).Returns(newViewFor);

        var adapter = A.Fake<INavigationViewAdapter>();

        var oldDialog = A.Fake<IDialog>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredModalStack.Push(oldVm);
        ns.ContentDialog = oldDialog;

        await ns.PushModal(newVm, null).FirstAsync();

        ns.MirroredModalStack.ToArray().Should().Equal(newVm, oldVm);

        A.CallTo(() => dm.AreModalDialogsActive).MustHaveHappened();
        A.CallTo(() => dm.Create(newViewFor)).MustHaveHappened();
        A.CallTo(() => newDialog.Hide()).MustNotHaveHappened();
        A.CallTo(() => newDialog.Show()).MustHaveHappened();
        A.CallTo(() => oldDialog.Hide()).MustHaveHappened();
    }

    [Test]
    public async Task Test_PushModal_NoVieeFound_ShouldFailAsync()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;

        var newDialog = A.Fake<IDialog>();
        var dm = A.Fake<IDialogManager>();
        A.CallTo(() => dm.AreModalDialogsActive).Returns(false);

        var oldVm = A.Fake<IViewModel>();
        var newVm = A.Fake<IViewModel>();
        var newViewFor = A.Fake<IViewFor>();
        var vl = A.Fake<IViewTypeLocator>();
        A.CallTo(() => vl.ResolveView<object>(newVm, null)).Returns(null);

        var adapter = A.Fake<INavigationViewAdapter>();

        var oldDialog = A.Fake<IDialog>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);
        ns.MirroredModalStack.Push(oldVm);
        ns.ContentDialog = oldDialog;

        var call = async () => await ns.PushModal(newVm, null).FirstAsync();
        var result = await call.Should().ThrowAsync<System.InvalidOperationException>();

        result.WithMessage(
            $"No view could be located for type '{newVm.GetType().FullName}', contract ''. Be sure Splat has an appropriate registration."
        );

        ns.MirroredModalStack.ToArray().Should().Equal(newVm, oldVm);

        A.CallTo(() => dm.AreModalDialogsActive).MustHaveHappened();
        A.CallTo(() => dm.Create(newViewFor)).MustNotHaveHappened();
        A.CallTo(() => newDialog.Hide()).MustNotHaveHappened();
        A.CallTo(() => newDialog.Show()).MustNotHaveHappened();
        A.CallTo(() => oldDialog.Hide()).MustNotHaveHappened();
    }

    [Test]
    public void Test_Dispose_ShouldDispose()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var dm = A.Fake<IDialogManager>();
        var vl = A.Fake<IViewTypeLocator>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);

        var call = () => ns.Dispose();

        call.Should().NotThrow();
    }

    [Test]
    public void Test_Dispose_WithAdapter_ShouldDispose()
    {
        var mainScheduler = RxApp.MainThreadScheduler;
        var bgScheduler = RxApp.TaskpoolScheduler;
        var dm = A.Fake<IDialogManager>();
        var vl = A.Fake<IViewTypeLocator>();
        var adapter = A.Fake<INavigationViewAdapter>();

        var ns = new NavigationService(mainScheduler, bgScheduler, vl, dm);
        ns.SetAdapter(adapter);

        var call = () => ns.Dispose();

        call.Should().NotThrow();
    }
}
