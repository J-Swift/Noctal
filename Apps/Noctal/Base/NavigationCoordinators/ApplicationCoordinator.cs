using Android.Content.Res;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Google.Android.Material.BottomNavigation;

namespace Noctal;

#if ANDROID
public class ApplicationCoordinator : BaseCoordinator
{
    public ApplicationCoordinator(MainActivity activity, FragmentFactory factory) : base(activity)
    {
        ChildCoordinators.Add(new StoriesCoordinator(activity, factory));
        ChildCoordinators.Add(new SearchCoordinator(activity));
        ChildCoordinators.Add(new AccountCoordinator(activity));
        ChildCoordinators.Add(new SettingsCoordinator(activity));
    }

    public void SetContentView()
    {
        var activity = Activity.Target()!;

        activity.SetContentView(Resource.Layout.activity_main);

        var theme = activity.CurrentTheme;

        var csl = new ColorStateList(new[]
        {
            new int[] { Android.Resource.Attribute.StateChecked },
            Array.Empty<int>(),
        },
        new int[]
        {
            theme.PrimaryColor.ToPlatform(),
            theme.OnBackgroundColor.ToPlatform(),
        });

        var toolbar = activity.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar)!;
        toolbar.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        toolbar.SetTitleTextColor(theme.OnBackgroundColor.ToPlatform());

        var navView = activity.FindViewById<Google.Android.Material.BottomNavigation.BottomNavigationView>(Resource.Id.nav_view)!;
        navView.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        navView.ItemTextColor = csl;
        navView.ItemIconTintList = csl;
        navView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;

        var container = activity.FindViewById<AndroidX.Fragment.App.FragmentContainerView>(Resource.Id.nav_host_fragment_activity_main)!;
        container.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
    }

    public override SubgraphEntry GetSubgraph()
    {
        throw new NotImplementedException();
    }

    public override async Task Start()
    {
        await base.Start();

        var activity = Activity.Target() ?? throw new ArgumentNullException(nameof(Activity));

        var toolbar = activity.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar)!;
        var navView = activity.FindViewById<Google.Android.Material.BottomNavigation.BottomNavigationView>(Resource.Id.nav_view)!;

        Nav = ((NavHostFragment)activity.SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment_activity_main)!).NavController;
        var navigator = (FragmentNavigator)Nav.NavigatorProvider.GetNavigator(Java.Lang.Class.FromType(typeof(FragmentNavigator)));

        var appGraph = ChildCoordinators.Select(it => it.GetSubgraph()).ToList();
        var mainGraph = new NavGraphBuilder(Nav.NavigatorProvider, appGraph[0].StartDestId, "main_graph");

        foreach (var subgraphConfig in appGraph)
        {
            foreach (var entry in subgraphConfig.SubItems)
            {
                if (entry is BasicNavEntry basic)
                {
                    var dest = DestFor(basic, navigator);
                    if (basic is TopLevelEntry top)
                    {
                        AddMenuItem(activity, navView.Menu, dest.Id, top.Label, top.IconResId);
                    }
                    mainGraph.AddDestination(dest);
                }
            }
        }
        var navGraph = (NavGraph)mainGraph.Build();

        Nav.DestinationChanged += (_, e) =>
        {
            int? checkedItem = null;
            foreach (var subgraphConfig in appGraph)
            {
                if (subgraphConfig.SubItems.Any(it => it.Id == e.Destination.Route))
                {
                    checkedItem = Nav.FindDestination(subgraphConfig.StartDestId).Id;
                    break;
                }
            }
            if (checkedItem is not null)
            {
                navView.Menu.FindItem((int)checkedItem)!.SetChecked(true);
            }
        };

        Nav.SetGraph(navGraph, null);

        activity.OnBackPressedDispatcher.AddCallback(activity, new BackPressedCallback(() =>
        {
            var prevEntry = Nav.PreviousBackStackEntry;
            var curEntry = Nav.CurrentBackStackEntry;
            if (prevEntry == null)
            {
                // empty back stack on home tab
                Activity.Target()?.Finish();
            }
            else if (navView.SelectedItemId == curEntry?.Destination?.Id && navView.SelectedItemId != Nav.Graph.StartDestinationId)
            {
                // on the root of the selected tab, but we arent on home
                navView.SelectedItemId = Nav.Graph.StartDestinationId;
            }
            else
            {
                // we are not at the root of a tab
                Nav.PopBackStack();
            }
        }));

        var appBarConfiguration = new AppBarConfiguration.Builder(navView.Menu).Build();
        NavigationUI.SetupWithNavController(toolbar, Nav, appBarConfiguration);
        NavigationUI.SetupWithNavController(navView, Nav);

        await Task.WhenAll(ChildCoordinators.Select(it => it.Start()));
    }

    private class BackPressedCallback : AndroidX.Activity.OnBackPressedCallback
    {
        private readonly Action Callback;

        public BackPressedCallback(Action callback) : base(true)
        {
            Callback = callback;
        }

        public override void HandleOnBackPressed()
        {
            Callback();
        }
    }

    private FragmentNavigator.Destination DestFor(BasicNavEntry entry, FragmentNavigator navigator)
    {
        var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, entry.Id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(entry.PageType)))
        {
            Label = entry.Label,
        };

        var res = (FragmentNavigator.Destination)resBuilder.Build();
        return res;
    }

    private void AddMenuItem(Android.Content.Context context, IMenu menu, int itemId, string label, int iconResId)
    {
        var item = menu.Add(IMenu.None, itemId, IMenu.None, label)!;
        var drawable = ContextCompat.GetDrawable(context, iconResId);
        item.SetIcon(drawable);
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
