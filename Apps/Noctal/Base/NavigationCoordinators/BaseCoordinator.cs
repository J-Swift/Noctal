namespace Noctal;

public abstract class BaseCoordinator
{
    protected readonly IList<BaseCoordinator> ChildCoordinators = new List<BaseCoordinator>();

    public virtual async Task Start()
    {
        await Task.Delay(0);
    }
}
