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

        // var vm = new FeedViewModel();
        // var page = new FeedView(vm);
        // page.OnItemSelected += OnFeedItemSelectedAsync;

        // NavigationPage.SetHasNavigationBar(page, false);
        // await NavPage.PushAsync(page, false);
        var page = new ContentPage {
            Content = new VerticalStackLayout {
                BackgroundColor = Colors.Green,
                // VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    new Label { Text = "One",  BackgroundColor = Colors.Yellow},
                    new Label { Text = "Two", BackgroundColor = Colors.Brown, VerticalOptions = LayoutOptions.FillAndExpand, },
                    new Label { Text = "Three", BackgroundColor = Colors.Red},
                }
            }
        };
        await NavPage.PushAsync(page, false);
    }

    private async void OnFeedItemSelectedAsync(object? sender, FeedView.FeedItemSelectedArgs e)
    {
        var vm = new FeedDetailViewModel(e.Item);
        var page = new FeedDetailView(vm);

        await NavPage.PushAsync(page, true);
    }
}
