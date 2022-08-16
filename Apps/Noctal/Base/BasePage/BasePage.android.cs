using Android.Content;
using Android.OS;
using Android.Views;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Noctal;

public abstract partial class BasePage<TViewModel> : ReactiveUI.AndroidX.ReactiveFragment<TViewModel> where TViewModel : class
{
    protected virtual void Initialize() { }
    protected abstract TViewModel CreateViewModel();
    protected abstract View CreateView(Context ctx);
    protected abstract void BindView(CompositeDisposable disposables);

    protected TViewModel SafeViewModel => ViewModel!;

    public BasePage()
    {
        ViewModel = CreateViewModel();
        Initialize();
        this.WhenActivated(disposables => BindView(disposables));
    }

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
    {
        return CreateView(RequireContext());
    }
}
