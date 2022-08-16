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
        Nav.PushViewController(page, true);
    }
}
#endif