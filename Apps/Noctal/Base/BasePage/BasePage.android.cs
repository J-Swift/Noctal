using Android.Content;
using Android.OS;
using Android.Views;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Noctal;

public abstract partial class BasePage<TViewModel> : ReactiveUI.AndroidX.ReactiveFragment<TViewModel> where TViewModel : class
{
    protected abstract TViewModel CreateViewModel();
    protected abstract View CreateView(Context ctx);
    protected abstract void BindView(CompositeDisposable disposables);

    protected TViewModel SafeViewModel => ViewModel!;

    public BasePage()
    {
        this.WhenActivated(disposables => BindView(disposables));
    }

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Arguments is Bundle args)
        {
            ReadArgs(args);
        }
    }

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
    {
        ViewModel = CreateViewModel();
        return CreateView(RequireContext());
    }

    protected virtual void ReadArgs(Bundle args) { }
}
