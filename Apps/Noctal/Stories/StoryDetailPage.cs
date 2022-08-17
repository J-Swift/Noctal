using ReactiveUI;
using System.Reactive.Disposables;

#if ANDROID
using Android.Content;
using Android.Views;
#endif

namespace Noctal.Stories;

#if ANDROID
public class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    private int StoryId { get; set; }
    protected override StoryDetailViewModel CreateViewModel() => new(StoryId, new StoriesService());
    private NoctalLabel LblTitle { get; set; } = null!;

    public StoryDetailPage(int storyId) : base()
    {
        StoryId = storyId;
    }

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
    }

    protected override View CreateView(Context context)
    {
        LblTitle = new NoctalLabel(context);
        return LblTitle;
    }
}
#elif IOS
public class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    private int StoryId { get; set; }
    protected override StoryDetailViewModel CreateViewModel() => new(StoryId, new StoriesService());
    private NoctalLabel LblTitle { get; set; } = null!;

    public StoryDetailPage(int storyId) : base()
    {
        StoryId = storyId;
    }

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
    }

    protected override UIView CreateView()
    {
        LblTitle = new NoctalLabel();
        return LblTitle;
    }
}
#endif
