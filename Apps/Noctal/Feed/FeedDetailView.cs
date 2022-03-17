using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Maui;

namespace Noctal;

public class FeedDetailView : ReactiveContentPage<FeedDetailViewModel>
{
    private NoctalLabel LblTitle { get; set; } = null!;

    public void Initialize()
    {
        Padding = 16;

        LblTitle = new NoctalLabel
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            FontSize = 24,
        };
        Content = LblTitle;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Title, v => v.LblTitle.Text)
                .DisposeWith(disposables);
        });
    }
}
