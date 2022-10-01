using ReactiveUI;
using System.Reactive.Disposables;

namespace Noctal;

public abstract class BasePage<TViewModel> : ReactiveViewController<TViewModel> where TViewModel : class
{
    protected abstract TViewModel CreateViewModel();
    protected abstract UIView CreateView();
    protected abstract void BindView(CompositeDisposable disposables);

    protected TViewModel SafeViewModel => ViewModel!;

    protected BasePage()
    {
        this.WhenActivated(disposables =>
        {
            BindView(disposables);
        });
    }

    public override void LoadView()
    {
        ViewModel = CreateViewModel();
        View = CreateView();
        View.BackgroundColor = SceneDelegate.Theme.BackgroundColor;
    }
}
