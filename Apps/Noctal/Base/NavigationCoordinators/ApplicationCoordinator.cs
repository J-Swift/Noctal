namespace Noctal;

public class ApplicationCoordinator : BaseCoordinator
{
    public Page RootPage { get; private set; }

    private NavigationPage NavPage { get; } = new NavigationPage();

    public ApplicationCoordinator()
    {
        RootPage = NavPage;
    }

    public override async Task Start()
    {
        await base.Start();

        var page = new FeedView();
        page.OnItemSelected += OnFeedItemSelectedAsync;
        page.ViewModel = new();
        page.Initialize();

        await NavPage.PushAsync(page, false);
    }

    private async void OnFeedItemSelectedAsync(object? sender, FeedView.FeedItemSelectedArgs e)
    {
        var page = new FeedDetailView();
        page.ViewModel = new(e.Item);
        page.Initialize();

        await NavPage.PushAsync(page, true);
    }
}
