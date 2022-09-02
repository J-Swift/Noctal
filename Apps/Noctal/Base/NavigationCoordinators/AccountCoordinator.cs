namespace Noctal;

#if IOS
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

