using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.Maui.Platform;

namespace Noctal;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (Theme.TryResolveAttribute(Resource.Attribute.maui_splash))
        {
            SetTheme(Resource.Style.Theme_Material3_DayNight);
        }

        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_main);
    }
}
