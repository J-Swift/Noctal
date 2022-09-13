using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace Noctal;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    static bool isNight = false;

    private ApplicationCoordinator Coordinator = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var factory = new FragmentFactory();
        SupportFragmentManager.FragmentFactory = factory;
        Coordinator = new ApplicationCoordinator(this, factory);

        isNight = !isNight;
        AppCompatDelegate.DefaultNightMode = isNight ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

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
