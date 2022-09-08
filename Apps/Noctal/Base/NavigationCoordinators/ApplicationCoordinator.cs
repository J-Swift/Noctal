namespace Noctal;

#if ANDROID
public class ApplicationCoordinator : BaseCoordinator
{
    public ApplicationCoordinator(MainActivity activity)
    {
        ChildCoordinators.Add(new StoriesCoordinator(activity));
        ChildCoordinators.Add(new SearchCoordinator());
        ChildCoordinators.Add(new AccountCoordinator());
        ChildCoordinators.Add(new SettingsCoordinator());
    }

    public override SubgraphEntry GetSubgraph()
    {
        throw new NotImplementedException();
    }

    public IList<SubgraphEntry> GetGraph()
    {
        return ChildCoordinators.Select(it => it.GetSubgraph()).ToList();
    }
}
#elif IOS
public class ApplicationCoordinator : BaseCoordinator
{
    public UIViewController RootView => TabController;
    private UITabBarController TabController { get; } = new UITabBarController();

    public override async Task Start()
    {
        await base.Start();

        var idx = 0;
        var addChild = (UIViewController page, string title, string icName) =>
        {
            var vc = new UIViewController { Title = title, };
            var image = UIImage.FromBundle(icName);
            page.TabBarItem = new UITabBarItem(title, image, idx++);
            TabController.AddChildViewController(page);
        };

        var c1 = new StoriesCoordinator();
        addChild(c1.RootPage, "Stories", "ic_home");
        ChildCoordinators.Add(c1);

        var c2 = new SearchCoordinator();
        addChild(c2.RootPage, "Search", "ic_search");
        ChildCoordinators.Add(c2);

        var c3 = new AccountCoordinator();
        addChild(c3.RootPage, "Account", "ic_person");
        ChildCoordinators.Add(c3);

        var c4 = new SettingsCoordinator();
        addChild(c4.RootPage, "Settings", "ic_settings");
        ChildCoordinators.Add(c4);

        await Task.WhenAll(ChildCoordinators.Select(it => it.Start()));
    }
}

//public class ApplicationCoordinator : BaseCoordinator
//{
//    public Page RootPage { get; private set; }

//    private NavigationPage NavPage { get; } = new NavigationPage();

//    public ApplicationCoordinator()
//    {
//        RootPage = NavPage;
//    }

//    public override async Task Start()
//    {
//        await base.Start();

//        // var vm = new FeedViewModel();
//        // var page = new FeedView(vm);
//        // page.OnItemSelected += OnFeedItemSelectedAsync;

//        // NavigationPage.SetHasNavigationBar(page, false);
//        // await NavPage.PushAsync(page, false);
//        var page = new ContentPage {
//            Content = new VerticalStackLayout {
//                BackgroundColor = Colors.Green,
//                // VerticalOptions = LayoutOptions.FillAndExpand,
//                Children = {
//                    new Label { Text = "One",  BackgroundColor = Colors.Yellow},
//                    new Label { Text = "Two", BackgroundColor = Colors.Brown, VerticalOptions = LayoutOptions.FillAndExpand, },
//                    new Label { Text = "Three", BackgroundColor = Colors.Red},
//                }
//            }
//        };
//        await NavPage.PushAsync(page, false);
//    }

//    private async void OnFeedItemSelectedAsync(object? sender, FeedView.FeedItemSelectedArgs e)
//    {
//        var vm = new FeedDetailViewModel(e.Item);
//        var page = new FeedDetailView(vm);

//        await NavPage.PushAsync(page, true);
//    }
//}

#endif