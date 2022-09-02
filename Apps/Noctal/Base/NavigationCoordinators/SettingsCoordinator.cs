namespace Noctal;

#if IOS
public class SettingsCoordinator : BaseCoordinator
{
    public UIViewController RootPage => NavController;
    private UINavigationController NavController { get; } = new();

    public override async Task Start()
    {
        await base.Start();

        var vc = new UIViewController { Title = "Settings", };
        vc.View = new NoctalLabel
        {
            Text = "Settings Page",
            TextAlignment = UITextAlignment.Center,
        };
        NavController.SetViewControllers(new[] { vc }, false);
    }
}
#endif

