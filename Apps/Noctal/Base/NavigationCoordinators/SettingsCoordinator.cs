using Noctal.Settings;

namespace Noctal;

#if ANDROID
public class SettingsCoordinator : BaseCoordinator
{
    public SettingsCoordinator(MainActivity activity) : base(activity) { }

    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_settings", "navigation_settings", new[]
        {
            new TopLevelEntry("navigation_settings", typeof(SettingsPage),"Settings", Resource.Drawable.ic_settings),
        });
    }
}
#elif IOS
public class SettingsCoordinator : BaseCoordinator
{
    public UIViewController RootPage => NavController;
    private UINavigationController NavController { get; } = new();

    public override async Task Start()
    {
        await base.Start();

        var page = new SettingsPage { Title = "Settings", };
        NavController.SetViewControllers(new[] { page }, false);
    }
}
#endif

