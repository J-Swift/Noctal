using Noctal.Search;

namespace Noctal;

#if ANDROID
public class SearchCoordinator : BaseCoordinator
{
    public SearchCoordinator(MainActivity activity) : base(activity) { }

    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_search", "navigation_search", new[]
        {
            new TopLevelEntry("navigation_search", typeof(SearchPage), "Search", Resource.Drawable.ic_search),
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

        var page = new SearchPage { Title = "Search", };
        NavController.SetViewControllers(new[] { page }, false);
    }
}
#endif

