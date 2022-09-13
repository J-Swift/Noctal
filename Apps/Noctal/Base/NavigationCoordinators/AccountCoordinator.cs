using Noctal.Account;

namespace Noctal;

#if ANDROID
public class AccountCoordinator : BaseCoordinator
{
    public AccountCoordinator(MainActivity activity) : base(activity) { }

    public override SubgraphEntry GetSubgraph()
    {
        return new SubgraphEntry("subnav_account", "navigation_account", new[]
        {
            new TopLevelEntry("navigation_account", typeof(AccountPage), "Account", Resource.Drawable.ic_person),
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

        var page = new AccountPage { Title = "Account", };
        NavController.SetViewControllers(new[] { page }, false);
    }
}
#endif

