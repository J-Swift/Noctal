using Android.Content.Res;
using Android.Views;
using AndroidX.Core.Content.Resources;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using Noctal.Stories;

namespace Noctal;

#if ANDROID
public class ApplicationCoordinator : BaseCoordinator
{
    public ApplicationCoordinator()
    {
        ChildCoordinators.Add(new StoriesCoordinator());
        ChildCoordinators.Add(new SearchCoordinator());
        ChildCoordinators.Add(new AccountCoordinator());
        ChildCoordinators.Add(new SettingsCoordinator());
    }
    //public static NavGraph CreateNavGraph(FragmentNavigator navigator)
    //{
    //    var destForTopLevel = (Type type, string id, string lbl, string icName) =>
    //    {
    //        var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(type)))
    //        {
    //            Label = lbl,
    //        };

    //        var res = (FragmentNavigator.Destination)resBuilder.Build();
    //        var item = navView.Menu.Add(IMenu.None, res.Id, IMenu.None, res.Label)!;

    //        var resId = resources.GetIdentifier(icName, "drawable", PackageName);
    //        var drawable = ResourcesCompat.GetDrawable(resources, resId, null)!;
    //        item.SetIcon(drawable);
    //        return res;
    //    };
    //    var destFor = (Type type, string id, string lbl) =>
    //    {
    //        var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(type)))
    //        {
    //            Label = lbl,
    //        };

    //        var res = (FragmentNavigator.Destination)resBuilder.Build();
    //        return res;
    //    };

    //    var topLevelDests = new NavDestination[] {
    //        destForTopLevel(typeof(StoriesPage), "navigation_stories", "Stories", "ic_home"),
    //        destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_search", "Search", "ic_search"),
    //        destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_account", "Account", "ic_person"),
    //        destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_settings", "Settings", "ic_settings"),
    //    };
    //    var dests = new NavDestination[] {
    //        destFor(typeof(StoryDetailPage), "navigation_story", "Stories"),
    //    };

    //    detailFragId = dests[0].Id;

    //    var graphBuilder = new NavGraphBuilder(navController.NavigatorProvider, topLevelDests[0].Route!, null);
    //    foreach (var dest in topLevelDests)
    //    {
    //        graphBuilder.AddDestination(dest);
    //    }
    //    foreach (var dest in dests)
    //    {
    //        graphBuilder.AddDestination(dest);
    //    }
    //    var graph = (NavGraph)graphBuilder.Build();
    //}

    public override SubgraphEntry GetSubgraph()
    {
        //var childGraphs = ChildCoordinators.Select(it => it.GetSubgraph());

        throw new NotImplementedException();
        //var entries = ChildCoordinators.Select(it => it.GetNavEntries());
        //return ChildCoordinators.Select(it => new SubgraphEntry(it.GetNavEntries())).ToList();

        //var

    }

    public IList<SubgraphEntry> GetGraph()
    {
        //var childGraphs = ChildCoordinators.Select(it => it.GetNavEntries());
        return ChildCoordinators.Select(it => it.GetSubgraph()).ToList();
    }
}
#elif IOS
public class ApplicationCoordinator : BaseCoordinator
{
    public UIViewController RootView => TabController;
    private UITabBarController TabController { get; } = new UITabBarController();

    public override async Task Start()
    {
        await base.Start();

        var idx = 0;
        var addChild = (UIViewController page, string title, string icName) =>
        {
            var vc = new UIViewController { Title = title, };
            var image = UIImage.FromBundle(icName);
            page.TabBarItem = new UITabBarItem(title, image, idx++);
            TabController.AddChildViewController(page);
        };

        var c1 = new StoriesCoordinator();
        addChild(c1.RootPage, "Stories", "ic_home");
        ChildCoordinators.Add(c1);

        var c2 = new SearchCoordinator();
        addChild(c2.RootPage, "Search", "ic_search");
        ChildCoordinators.Add(c2);

        var c3 = new AccountCoordinator();
        addChild(c3.RootPage, "Account", "ic_person");
        ChildCoordinators.Add(c3);

        var c4 = new SettingsCoordinator();
        addChild(c4.RootPage, "Settings", "ic_settings");
        ChildCoordinators.Add(c4);

        await Task.WhenAll(ChildCoordinators.Select(it => it.Start()));
    }
}

//public class ApplicationCoordinator : BaseCoordinator
//{
//    public Page RootPage { get; private set; }

//    private NavigationPage NavPage { get; } = new NavigationPage();

//    public ApplicationCoordinator()
//    {
//        RootPage = NavPage;
//    }

//    public override async Task Start()
//    {
//        await base.Start();

//        // var vm = new FeedViewModel();
//        // var page = new FeedView(vm);
//        // page.OnItemSelected += OnFeedItemSelectedAsync;

//        // NavigationPage.SetHasNavigationBar(page, false);
//        // await NavPage.PushAsync(page, false);
//        var page = new ContentPage {
//            Content = new VerticalStackLayout {
//                BackgroundColor = Colors.Green,
//                // VerticalOptions = LayoutOptions.FillAndExpand,
//                Children = {
//                    new Label { Text = "One",  BackgroundColor = Colors.Yellow},
//                    new Label { Text = "Two", BackgroundColor = Colors.Brown, VerticalOptions = LayoutOptions.FillAndExpand, },
//                    new Label { Text = "Three", BackgroundColor = Colors.Red},
//                }
//            }
//        };
//        await NavPage.PushAsync(page, false);
//    }

//    private async void OnFeedItemSelectedAsync(object? sender, FeedView.FeedItemSelectedArgs e)
//    {
//        var vm = new FeedDetailViewModel(e.Item);
//        var page = new FeedDetailView(vm);

//        await NavPage.PushAsync(page, true);
//    }
//}

#endif