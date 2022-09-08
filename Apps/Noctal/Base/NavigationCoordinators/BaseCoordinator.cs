namespace Noctal;

public abstract partial class BaseCoordinator
{
    protected readonly IList<BaseCoordinator> ChildCoordinators = new List<BaseCoordinator>();

    public virtual async Task Start()
    {
        await Task.Delay(0);
    }
}

#if ANDROID
public record SubgraphEntry(string SubgraphId, string StartDestId, IList<NavItem> SubItems);
public abstract record NavItem();
public record BasicNavEntry(string Id, Type PageType, string Label) : NavItem();
public record TopLevelEntry(string Id, Type PageType, string Label, int IconResId) : BasicNavEntry(Id, PageType, Label);

public abstract partial class BaseCoordinator
{
    //public abstract IList<NavItem> GetNavEntries();
    public abstract SubgraphEntry GetSubgraph();

    public BaseCoordinator()
    {

    }
}
#endif
