namespace Noctal;

#if IOS
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

