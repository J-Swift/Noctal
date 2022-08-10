using ReactiveUI;
using ReactiveUI.Maui;
using System.Reactive.Disposables;

namespace Noctal;

public class FeedDetailView : ReactiveContentPage<FeedDetailViewModel>
{
    private NoctalLabel LblTitle { get; }
    private NoctalLabel LblUrl { get; }
    private NoctalLabel LblScore { get; }
    private NoctalLabel LblSubmitter { get; }
    private NoctalLabel LblTimeAgo { get; }

    public FeedDetailView(FeedDetailViewModel vm)
    {
        ViewModel = vm;

        Padding = 24;

        var grid = new Grid();

        var sv = new VerticalStackLayout { Spacing = 16, };

        LblTitle = new NoctalLabel
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            FontSize = 24,
        };
        sv.Add(LblTitle);

        LblUrl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
        sv.Add(new HorizontalStackLayout
        {
            Spacing = 4,
            Children = {
                new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Black.WithAlpha(0.1f), },
                LblUrl,
            },
        });

        LblScore = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
        LblSubmitter = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
        LblTimeAgo = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
        sv.Add(new HorizontalStackLayout
        {
            Spacing = 4,
            Children = {
                new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Red.WithAlpha(0.1f), },
                LblScore,
                new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                LblSubmitter,
                new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                LblTimeAgo,
            },
        });

        grid.Add(sv);

        Content = grid;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Title, v => v.LblTitle.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.Url, v => v.LblUrl.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.Score, v => v.LblScore.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.Submitter, v => v.LblSubmitter.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.TimeAgo, v => v.LblTimeAgo.Text)
                .DisposeWith(disposables);
        });
    }
}
