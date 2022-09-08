using Android.App;
using Android.Content;
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

        var mainGraph = new NavGraphBuilder(navController.NavigatorProvider, "subnav_stories", "main_graph");

        foreach (var subgraphConfig in appGraph)
        {
            var f = new GraphBuilder(this, subgraphConfig, navigator, topLevelDests);
            NavGraphBuilderKt.Navigation(mainGraph, subgraphConfig.StartDestId, subgraphConfig.SubgraphId, f);
            subGraphs.Add(subgraphConfig.SubgraphId);
        }
        var b = (NavGraph)mainGraph.Build();

        navController.SetGraph(b, null);

        foreach (var (t, s) in topLevelDests.Zip(subGraphs))
        {
            var drawable = ContextCompat.GetDrawable(this, t.IconResId);
            var id = navController.FindDestination(s).Id;
            var item = navView.Menu.Add(IMenu.None, id, IMenu.None, t.Label)!;
            Console.WriteLine($"JIMMY menu item [s {s}] [id {id}] [tid {t.Id}]");
            item.SetIcon(drawable);
        }

        var appBarConfiguration = new AppBarConfiguration.Builder(navView.Menu).Build();
        NavigationUI.SetupWithNavController(toolbar, navController, appBarConfiguration);
        NavigationUI.SetupWithNavController(navView, navController);
    }
}

class GraphBuilder : Java.Lang.Object, Kotlin.Jvm.Functions.IFunction1
{
    private readonly SubgraphEntry Subgraph;
    private readonly Android.Content.Context Context;
    private readonly FragmentNavigator FragmentNavigator;
    private readonly IList<TopLevelEntry> TopLevelDests;

    public GraphBuilder(Android.Content.Context context, SubgraphEntry subgraph, FragmentNavigator fragmentNavigator, IList<TopLevelEntry> topLevelDests)
    {
        Context = context;
        Subgraph = subgraph;
        FragmentNavigator = fragmentNavigator;
        TopLevelDests = topLevelDests;
    }

    public Java.Lang.Object? Invoke(Java.Lang.Object? p0)
    {
        var destForTopLevel = (TopLevelEntry e) =>
        {
            var resBuilder = new FragmentNavigatorDestinationBuilder(FragmentNavigator, e.Id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(e.PageType)))
            {
                Label = e.Label,
            };

            var res = (FragmentNavigator.Destination)resBuilder.Build();
            return res;
        };
        var destFor = (BasicNavEntry e) =>
        {
            var resBuilder = new FragmentNavigatorDestinationBuilder(FragmentNavigator, e.Id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(e.PageType)))
            {
                Label = e.Label,
            };

            var res = (FragmentNavigator.Destination)resBuilder.Build();
            return res;
        };

        var graphBuilder = (NavGraphBuilder)p0!;

        foreach (var entry in Subgraph.SubItems)
        {
            switch (entry)
            {
                case TopLevelEntry topLevel:
                    {
                        var dest = destForTopLevel(topLevel);
                        graphBuilder.AddDestination(dest);
                        TopLevelDests.Add(topLevel);
                        break;
                    }
                case BasicNavEntry basic:
                    {
                        var dest = destFor(basic);
                        graphBuilder.AddDestination(dest);
                        break;
                    }
                default:
                    throw new ArgumentException($"unknown type: [{entry}]");
            }
        }

        return graphBuilder;
    }
}
