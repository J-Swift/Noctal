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
public abstract record NavItem(string Id);
public record BasicNavEntry(string Id, Type PageType, string Label) : NavItem(Id);
public record TopLevelEntry(string Id, Type PageType, string Label, int IconResId) : BasicNavEntry(Id, PageType, Label);

public abstract partial class BaseCoordinator
{
    protected readonly WeakReference<MainActivity> Activity;
    protected static AndroidX.Navigation.NavController Nav { get; set; } = null!;

    //public abstract IList<NavItem> GetNavEntries();
    public abstract SubgraphEntry GetSubgraph();

    public BaseCoordinator(MainActivity activity)
    {
        Activity = new(activity);
    }
}
#endif

public static class WeakRefExtensions
{
    public static TItem? Target<TItem>(this WeakReference<TItem> weakRef) where TItem : class
    {
        if (weakRef.TryGetTarget(out var target))
        {
            return target;
        }

        return null;
    }
}

