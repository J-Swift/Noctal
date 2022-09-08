using Noctal.Stories;
using System.Reactive;
using System.Reactive.Linq;

#if ANDROID
using Android.OS;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using static Noctal.FragmentFactory;
#endif

namespace Noctal;

#if ANDROID
// https://damienaicheh.github.io/xamarin/xamarin.android/2018/05/26/xamarin-android-parcelable-en.html
public class StoriesCoordinator : BaseCoordinator, IFragmentFactoryAfterCreateListener
{
    private readonly MainActivity Activity;

    public StoriesCoordinator(MainActivity activity)
    {
        Activity = activity;
        activity.Factory.RegisterAfterCreateListener(typeof(StoriesPage), this);
    }

    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_stories", StoriesPage.NAVIGATION_ROUTE, new[]
        {
            new TopLevelEntry(StoriesPage.NAVIGATION_ROUTE, typeof(StoriesPage), "Stories", Resource.Drawable.ic_home),
            new BasicNavEntry(StoryDetailPage.NAVIGATION_ROUTE, typeof(StoryDetailPage), "Story"),
        });
    }

    public void AfterCreateFragment(Fragment fragment)
    {
        switch (fragment)
        {
            case StoriesPage stories:
                {
                    var _ = new MyObserver<StoriesPage.EventArgs>(
                        stories.Lifecycle,
                        Observable.FromEventPattern<StoriesPage.EventArgs>(ev => stories.OnItemSelected += ev, ev => stories.OnItemSelected -= ev),
                        item => ShowStoriesDetailPage(item.SelectedItem.Id)
                    );
                    break;
                }
        }
    }

    private void ShowStoriesDetailPage(int storyId)
    {
        var nav = Activity.Nav;
        var action = StoryDetailPage.SafeNav(nav, storyId);
        nav.Navigate(action.DestId, action.DestArgs);
    }
}

class MyObserver<T> : Java.Lang.Object, IDefaultLifecycleObserver, ILifecycleObserver
{
    private readonly IObservable<EventPattern<T>> Evt;
    private IDisposable? Subscription = null;

    public MyObserver(Lifecycle lifecycle, IObservable<EventPattern<T>> evt, Action<T> callback)
    {
        Evt = evt.Do(it => callback(it.EventArgs));

        lifecycle.AddObserver(this);
    }

    public void OnCreate(ILifecycleOwner? p0) { }
    public void OnStart(ILifecycleOwner? p0) { }
    public void OnResume(ILifecycleOwner? p0)
    {
        Subscription = Evt.Subscribe();
    }

    public void OnPause(ILifecycleOwner? p0)
    {
        Subscription?.Dispose();
        Subscription = null;
    }
    public void OnStop(ILifecycleOwner? p0) { }
    public void OnDestroy(ILifecycleOwner? p0) { }
}
#elif IOS
public class StoriesCoordinator : BaseCoordinator
{
    public UIViewController RootPage => Nav;
    private readonly UINavigationController Nav = new UINavigationController();

    public override async Task Start()
    {
        await Task.Delay(0);

        var page = new StoriesPage { Title = "Stories" };
        page.OnItemSelected += OnItemSelected;
        Nav.PushViewController(page, true);
    }

    private void OnItemSelected(object? sender, StoriesPage.EventArgs e)
    {
        var page = new StoryDetailPage(e.SelectedItem.Id);
        Nav.PushViewController(page, true);
    }
}
#endif
