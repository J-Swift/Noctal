using Noctal.Stories;

namespace Noctal;

#if IOS
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
