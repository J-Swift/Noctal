namespace Noctal;

#if ANDROID
public class SearchCoordinator : BaseCoordinator
{
    public SearchCoordinator(MainActivity activity) : base(activity) { }

    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_search", "navigation_search", new[]
        {
            new TopLevelEntry("navigation_search", typeof(AndroidX.Fragment.App.Fragment), "Search", Resource.Drawable.ic_search),
        });
    }
}
#elif IOS
public class SearchCoordinator : BaseCoordinator
{
    public UIViewController RootPage => NavController;
    private UINavigationController NavController { get; } = new();

    public override async Task Start()
    {
        await base.Start();

        var vc = new UIViewController { Title = "Search", };
        vc.View = new NoctalLabel
        {
            Text = "Search Page",
            TextAlignment = UITextAlignment.Center,
        };
        NavController.SetViewControllers(new[] { vc }, false);
    }
}
#endif

