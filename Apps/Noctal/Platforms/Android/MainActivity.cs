using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Noctal.UI.Theming;

namespace Noctal;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    static bool isNight = false;

    public ITheme CurrentTheme { get; private set; } = null!;
    private ApplicationCoordinator Coordinator = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        isNight = !isNight;
        CurrentTheme = isNight ? new DarkTheme() : new LightTheme();
        AppCompatDelegate.DefaultNightMode = isNight ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

        var factory = new FragmentFactory();
        SupportFragmentManager.FragmentFactory = factory;
        Coordinator = new ApplicationCoordinator(this, factory);

        if (Theme.TryResolveAttribute(Resource.Attribute.maui_splash))
        {
            SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar);
        }

        base.OnCreate(savedInstanceState);

        Coordinator.SetContentView();
    }

    protected override void OnPostCreate(Bundle? savedInstanceState)
    {
        base.OnPostCreate(savedInstanceState);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Coordinator.Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
}
