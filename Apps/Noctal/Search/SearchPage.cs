using ReactiveUI;
using System.Reactive.Disposables;

#if ANDROID
using Android.Content;
using Android.Views;
#endif

namespace Noctal.Search;

public partial class SearchPage : BasePage<SearchViewModel>
{
    protected override SearchViewModel CreateViewModel() => new();
}

#if ANDROID
public partial class SearchPage
{
    private NoctalLabel LblText { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Text, v => v.LblText.Text)
            .DisposeWith(disposables);
    }

    protected override View CreateView(Context ctx)
    {
        LblText = new NoctalLabel(ctx)
        {
            Gravity = GravityFlags.Center,
            //TextAlignment = UITextAlignment.Center,
        };
        return LblText;
    }
}
#elif IOS
public partial class SearchPage
{
    private NoctalLabel LblText { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Text, v => v.LblText.Text)
            .DisposeWith(disposables);
    }

    protected override UIView CreateView()
    {
        LblText = new NoctalLabel
        {
            TextAlignment = UITextAlignment.Center,
        };
        return LblText;
    }
}
#endif
