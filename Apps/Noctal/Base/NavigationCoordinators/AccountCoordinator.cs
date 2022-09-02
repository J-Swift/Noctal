namespace Noctal;

#if ANDROID
public class AccountCoordinator : BaseCoordinator
{
    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_account", "navigation_account", new[]
        {
            new TopLevelEntry("navigation_account", typeof(AndroidX.Fragment.App.Fragment), "Account", Resource.Drawable.ic_person),
        });
    }
}
#elif IOS
public class AccountCoordinator : BaseCoordinator
{
    public UIViewController RootPage => NavController;
    private UINavigationController NavController { get; } = new();

    public override async Task Start()
    {
        await base.Start();

        var vc = new UIViewController { Title = "Account", };
        vc.View = new NoctalLabel
        {
            Text = "Account Page",
            TextAlignment = UITextAlignment.Center,
        };
        NavController.SetViewControllers(new[] { vc }, false);
    }
}
#endif

