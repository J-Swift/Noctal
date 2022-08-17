using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Google.Android.Material.BottomNavigation;
using Noctal.Stories;
using Noctal.Stories.Models;
using Noctal.UI.Theming;
using static Noctal.FragmentFactory;

namespace Noctal;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : AppCompatActivity, IFragmentFactoryListener
{
    static bool isNight = false;

    private int detailFragId;

    private FragmentFactory Factory = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Factory = new FragmentFactory();
        SupportFragmentManager.FragmentFactory = Factory;
        Factory.RegisterListener(typeof(StoriesPage), this);
        Factory.RegisterListener(typeof(StoryDetailPage), this);

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
        var navHost = (NavHostFragment)SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment_activity_main)!;
        var navController = navHost.NavController;
        var navigator = (FragmentNavigator)navController.NavigatorProvider.GetNavigator(Java.Lang.Class.FromType(typeof(FragmentNavigator)));

        var destForTopLevel = (Type type, string id, string lbl, string icName) =>
        {
            var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(type)))
            {
                Label = lbl,
            };

            var res = (FragmentNavigator.Destination)resBuilder.Build();
            var item = navView.Menu.Add(IMenu.None, res.Id, IMenu.None, res.Label)!;

            var resId = resources.GetIdentifier(icName, "drawable", PackageName);
            var drawable = ResourcesCompat.GetDrawable(resources, resId, null)!;
            item.SetIcon(drawable);
            return res;
        };
        var destFor = (Type type, string id, string lbl) =>
        {
            var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(type)))
            {
                Label = lbl,
            };

            var res = (FragmentNavigator.Destination)resBuilder.Build();
            return res;
        };

        var topLevelDests = new NavDestination[] {
            destForTopLevel(typeof(StoriesPage), "navigation_stories", "Stories", "ic_home"),
            destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_search", "Search", "ic_search"),
            destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_account", "Account", "ic_person"),
            destForTopLevel(typeof(AndroidX.Fragment.App.Fragment), "navigation_settings", "Settings", "ic_settings"),
        };
        var dests = new NavDestination[] {
            destFor(typeof(StoryDetailPage), "navigation_story", "Stories"),
        };

        detailFragId = dests[0].Id;

        var graphBuilder = new NavGraphBuilder(navController.NavigatorProvider, topLevelDests[0].Route!, null);
        foreach (var dest in topLevelDests)
        {
            graphBuilder.AddDestination(dest);
        }
        foreach (var dest in dests)
        {
            graphBuilder.AddDestination(dest);
        }
        var graph = (NavGraph)graphBuilder.Build();

        navController.SetGraph(graph, null);

        var appBarConfiguration = new AppBarConfiguration(topLevelDests.Select(it => it.Id).ToArray(), null, null, null);
        NavigationUI.SetupWithNavController(toolbar, navController, appBarConfiguration);
        NavigationUI.SetupWithNavController(navView, navController);
    }

    private StoriesFeedItem? SelectedItem;
    public AndroidX.Fragment.App.Fragment? OnCreateFragment(string className)
    {
        if (className == Java.Lang.Class.FromType(typeof(StoriesPage)).Name)
        {
            var storiesFrag = new StoriesPage();
            storiesFrag.OnItemSelected += OnStorySelected;
            return storiesFrag;
        }
        else if (className == Java.Lang.Class.FromType(typeof(StoryDetailPage)).Name)
        {
            var storyFrag = new StoryDetailPage(SelectedItem!.Id);
            return storyFrag;
        }
        return null;
    }

    private void OnStorySelected(object? sender, StoriesPage.EventArgs e)
    {
        Console.WriteLine($"Story Selected [{e.SelectedItem}]");
        SelectedItem = e.SelectedItem;

        var navCont = NavHostFragment.FindNavController(sender as AndroidX.Fragment.App.Fragment);
        var nav = new ActionOnlyNavDirections(detailFragId);
        navCont.Navigate(nav);
    }
}
