namespace Noctal;

public abstract class BaseCoordinator
{
    public virtual async Task Start()
    {
        await Task.Delay(0);
    }
}
