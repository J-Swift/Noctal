using Noctal.Stories;

namespace Noctal;

#if ANDROID
public class StoriesCoordinator : BaseCoordinator
{
    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_stories", "navigation_home", new[]
        {
            new TopLevelEntry("navigation_home", typeof(StoriesPage), "Stories", Resource.Drawable.ic_home),
            new BasicNavEntry("navigation_story", typeof(StoryDetailPage), "Story"),
        });
    }
}
#elif IOS
public class StoriesCoordinator : BaseCoordinator
{
    public UIViewController RootPage => Nav;
    private readonly UINavigationController Nav = new UINavigationController();

    public override async Task Start()
    {
        await Task.Delay(0);

        var page = new StoriesPage { Title = "Stories" };
        page.OnItemSelected += OnItemSelected;
        Nav.PushViewController(page, true);
    }

    private void OnItemSelected(object? sender, StoriesPage.EventArgs e)
    {
        var page = new StoryDetailPage(e.SelectedItem.Id);
        Nav.PushViewController(page, true);
    }
}
#endif
