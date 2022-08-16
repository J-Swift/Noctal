using ReactiveUI;
using System.Reactive.Disposables;

namespace Noctal;

public abstract class BasePage<TViewModel> : ReactiveUI.ReactiveViewController<TViewModel> where TViewModel : class
{
    protected virtual void Initialize() { }
    protected abstract TViewModel CreateViewModel();
    protected abstract UIView CreateView();
    protected abstract void BindView(CompositeDisposable disposables);

    protected TViewModel SafeViewModel => ViewModel!;

    public BasePage()
    {
        ViewModel = CreateViewModel();
        Initialize();
        this.WhenActivated(disposables => BindView(disposables));
    }

    public override void LoadView()
    {
        View = CreateView();
    }
}
