using Foundation;
using Noctal.UI.Theming;

namespace Noctal;

[Register(nameof(SceneDelegate))]
public class SceneDelegate : UIWindowSceneDelegate
{
    public static AdaptiveTheme Theme { get; private set; } = null!;

    public override UIWindow? Window { get; set; }

    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is UIWindowScene windowScene)
        {
            Theme = new AdaptiveTheme(new LightTheme(), new DarkTheme(), false);
            ApplyTheme();

            Window = new UIWindow(windowScene)
            {
                BackgroundColor = Theme.BackgroundColor,
            };
            var appCoordinator = new ApplicationCoordinator();

            Window.RootViewController = appCoordinator.RootView;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            appCoordinator.Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Window.MakeKeyAndVisible();
        }
    }

    private void ApplyTheme()
    {
        var navigationBarAppearance = new UINavigationBarAppearance();
        navigationBarAppearance.BackgroundColor = Theme.BackgroundColor;
        navigationBarAppearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = Theme.OnBackgroundColor };
        UINavigationBar.Appearance.StandardAppearance = UINavigationBar.Appearance.ScrollEdgeAppearance = navigationBarAppearance;
        UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

        UITabBar.Appearance.SelectedImageTintColor = Theme.PrimaryColor;
        UITabBar.Appearance.UnselectedItemTintColor = Theme.OnBackgroundColor;
    }
}
