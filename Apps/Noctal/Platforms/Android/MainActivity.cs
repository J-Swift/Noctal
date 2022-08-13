using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;
using AndroidX.Core.Graphics.Drawable;
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
    protected override void OnCreate(Bundle? savedInstanceState)
    {
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
        var container = FindViewById<AndroidX.Fragment.App.FragmentContainerView>(Resource.Id.nav_host_fragment_activity_main)!;
        container.SetBackgroundColor(theme.BackgroundColor.ToPlatform());
        var navHost = (NavHostFragment)SupportFragmentManager.FindFragmentById(Resource.Id.nav_host_fragment_activity_main)!;
        var navController = navHost.NavController;
        var navigator = (FragmentNavigator)navController.NavigatorProvider.GetNavigator(Java.Lang.Class.FromType(typeof(FragmentNavigator)));
        //var dialogNavigator = (DialogFragmentNavigator)navController.NavigatorProvider.GetNavigator(Java.Lang.Class.FromType(typeof(DialogFragmentNavigator)));

        var destFor = (Type type, string id, string lbl, string icName) =>
        {
            var resBuilder = new FragmentNavigatorDestinationBuilder(navigator, id, Kotlin.Jvm.JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(type)))
            {
                Label = lbl,
            };

            var res = (FragmentNavigator.Destination)resBuilder.Build();
            var item = navView.Menu.Add(IMenu.None, res.Id, IMenu.None, res.Label)!;

            var resId = resources.GetIdentifier(icName, "drawable", PackageName);
            var drawable = ResourcesCompat.GetDrawable(resources, resId, null)!;
            var mutDrawable = DrawableCompat.Wrap(drawable);
            DrawableCompat.SetTintMode(mutDrawable, PorterDuff.Mode.SrcIn!);
            DrawableCompat.SetTintList(mutDrawable, csl);
            item.SetIcon(mutDrawable);
            return res;
        };

        var dests = new NavDestination[] {
            destFor(typeof(AndroidX.Fragment.App.Fragment), "navigation_home", "Home", "ic_home"),
            destFor(typeof(AndroidX.Fragment.App.Fragment), "navigation_search", "Search", "ic_search"),
            destFor(typeof(AndroidX.Fragment.App.Fragment), "navigation_account", "Account", "ic_person"),
            destFor(typeof(AndroidX.Fragment.App.Fragment), "navigation_settings", "Settings", "ic_settings"),
        };

        var graphBuilder = new NavGraphBuilder(navController.NavigatorProvider, dests[0].Route!, null);
        foreach (var dest in dests)
        {
            graphBuilder.AddDestination(dest);
        }
        var graph = (NavGraph)graphBuilder.Build();

        navController.SetGraph(graph, null);

        var appBarConfiguration = new AppBarConfiguration(dests.Select(it => it.Id).ToArray(), null, null, null);
        NavigationUI.SetupWithNavController(toolbar, navController, appBarConfiguration);
        NavigationUI.SetupWithNavController(navView, navController);
    }
}
