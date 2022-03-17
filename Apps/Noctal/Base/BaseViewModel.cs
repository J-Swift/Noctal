using System.Reactive.Disposables;
using ReactiveUI;

namespace Noctal;

public abstract class BaseViewModel : ReactiveObject, IDisposable
{
    protected CompositeDisposable Subscriptions { get; } = new();

    // IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Subscriptions.Dispose();
        }
    }
}
