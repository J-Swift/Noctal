using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Google.Android.Material.BottomNavigation;
using Noctal.UI.Theming;

namespace Noctal;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    static bool isNight = false;

    public FragmentFactory Factory { get; private set; } = null!;
    public NavController Nav => ((NavHostFragment)SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment_activity_main)!).NavController;

    private ApplicationCoordinator Coordinator = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Factory = new FragmentFactory();
        SupportFragmentManager.FragmentFactory = Factory;
        Coordinator = new ApplicationCoordinator(this);

        isNight = !isNight;
        AppCompatDelegate.DefaultNightMode = isNight ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

        if (Theme.TryResolveAttribute(Resource.Attribute.maui_splash))
        {
            SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar);
        }

        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_main);
    }

    protected override void OnPostCreate(Bundle? savedInstanceState)
    {
        base.OnPostCreate(savedInstanceState);

        var resources = Resources!;
        var isNight = resources.Configuration!.IsNightModeActive;
        ITheme theme = isNight ? new DarkTheme() : new LightTheme();

        var csl = new ColorStateList(new[]
        {
            new int[] {Android.Resource.Attribute.StateChecked },
            Array.Empty<int>(),
        },
        new int[]
        {
            theme.PrimaryColor.ToPlatform(),
            theme.OnBackgroundColor.ToPlatform(),
        });

        var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar)!;
        toolbar.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        toolbar.SetTitleTextColor(theme.OnBackgroundColor.ToPlatform());
        //SetSupportActionBar(toolbar);

        var navView = FindViewById<Google.Android.Material.BottomNavigation.BottomNavigationView>(Resource.Id.nav_view)!;
        navView.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        navView.ItemTextColor = csl;
        navView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;
        navView.ItemIconTintList = csl;
        var container = FindViewById<AndroidX.Fragment.App.FragmentContainerView>(Resource.Id.nav_host_fragment_activity_main)!;
        container.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        var navController = Nav;
        var navigator = (FragmentNavigator)navController.NavigatorProvider.GetNavigator(Java.Lang.Class.FromType(typeof(FragmentNavigator)));

        var applicationCoord = Coordinator;
        var appGraph = applicationCoord.GetGraph();

        var subGraphs = new List<string>();
        var topLevelDests = new List<TopLevelEntry>();

        var mainGraph = new NavGraphBuilder(navController.NavigatorProvider, appGraph[0].StartDestId, "main_graph");

        foreach (var subgraphConfig in appGraph)
        {
            foreach (var entry in subgraphConfig.SubItems)
            {
                if (entry is BasicNavEntry basic)
                {
                    var dest = DestFor(basic, navigator);
                    if (basic is TopLevelEntry top)
                    {
                        AddMenuItem(navView.Menu, dest.Id, top.Label, top.IconResId);
                    }
                    mainGraph.AddDestination(dest);
                }
            }
        }
        var navGraph = (NavGraph)mainGraph.Build();

        navController.DestinationChanged += (_, e) =>
        {
            int? checkedItem = null;
            foreach (var subgraphConfig in appGraph)
            {
                if (subgraphConfig.SubItems.Any(it => it.Id == e.Destination.Route))
                {
                    checkedItem = navController.FindDestination(subgraphConfig.StartDestId).Id;
                    break;
                }
            }
            if (checkedItem is not null)
            {
                navView.Menu.FindItem((int)checkedItem)!.SetChecked(true);
            }
        };

        navController.SetGraph(navGraph, null);

        OnBackPressedDispatcher.AddCallback(this, new BackPressedCallback(() =>
        {
            var prevEntry = navController.PreviousBackStackEntry;
            var curEntry = navController.CurrentBackStackEntry;
            if (prevEntry == null)
            {
                // empty back stack on home tab
                Finish();
            }
            else if (navView.SelectedItemId == curEntry?.Destination?.Id && navView.SelectedItemId != navController.Graph.StartDestinationId)
            {
                // on the root of the selected tab, but we arent on home
                navView.SelectedItemId = navController.Graph.StartDestinationId;
            }
            else
            {
                // we are not at the root of a tab
                navController.PopBackStack();
            }
        }));

        var appBarConfiguration = new AppBarConfiguration.Builder(navView.Menu).Build();
        NavigationUI.SetupWithNavController(toolbar, navController, appBarConfiguration);
        NavigationUI.SetupWithNavController(navView, navController);
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

    private void AddMenuItem(IMenu menu, int itemId, string label, int iconResId)
    {
        var item = menu.Add(IMenu.None, itemId, IMenu.None, label)!;
        var drawable = ContextCompat.GetDrawable(this, iconResId);
        item.SetIcon(drawable);
    }
}
