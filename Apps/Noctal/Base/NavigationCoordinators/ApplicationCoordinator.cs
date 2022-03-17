namespace Noctal;

public class ApplicationCoordinator : BaseCoordinator
{
    public Page RootPage { get; private set; }

    public ApplicationCoordinator()
    {
        var page = new FeedView();
        RootPage = page;
    }
}
