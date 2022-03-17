using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Maui;
using Noctal.Models;

namespace Noctal;

public class FeedView : ReactiveContentPage<FeedViewModel>
{
    public record FeedItemSelectedArgs(FeedItem Item);
    public event EventHandler<FeedItemSelectedArgs>? OnItemSelected;

    private CollectionView Feed { get; set; } = null!;

    public void Initialize()
    {
        Feed = new CollectionView();

        var dt = new DataTemplate(() => new FeedCell());
        dt.SetBinding(FeedCell.ItemNumberProperty, nameof(FeedItem.Index));
        dt.SetBinding(FeedCell.UrlTextProperty, nameof(FeedItem.Url));
        dt.SetBinding(FeedCell.ArticleTitleProperty, nameof(FeedItem.Title));
        dt.SetBinding(FeedCell.SubmitterNameProperty, nameof(FeedItem.Submitter));
        dt.SetBinding(FeedCell.TimeAgoProperty, nameof(FeedItem.TimeAgo));
        dt.SetBinding(FeedCell.ScoreProperty, nameof(FeedItem.Score));
        dt.SetBinding(FeedCell.NumCommentsProperty, nameof(FeedItem.NumComments));
        Feed.ItemTemplate = dt;

        Feed.SelectionMode = SelectionMode.Single;
        Feed.SelectionChanged += (sender, args) =>
        {
            if (args.CurrentSelection?.Any() == true && args.CurrentSelection[0] is FeedItem item)
            {
                OnItemSelected?.Invoke(this, new FeedItemSelectedArgs(item));
                Feed.SelectedItem = null;
            }
        };

        var g = new Grid();
        g.Add(Feed);
        Content = g;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Items, v => v.Feed.ItemsSource)
                .DisposeWith(disposables);
        });
    }

    private class FeedCell : ContentView
    {
        public static readonly BindableProperty ItemNumberProperty = BindableProperty.Create(nameof(ItemNumber), typeof(int), typeof(FeedCell), 0);
        public static readonly BindableProperty UrlTextProperty = BindableProperty.Create(nameof(UrlText), typeof(string), typeof(FeedCell), "");
        public static readonly BindableProperty ArticleTitleProperty = BindableProperty.Create(nameof(ArticleTitle), typeof(string), typeof(FeedCell), "");
        public static readonly BindableProperty SubmitterNameProperty = BindableProperty.Create(nameof(SubmitterName), typeof(string), typeof(FeedCell), "");
        public static readonly BindableProperty TimeAgoProperty = BindableProperty.Create(nameof(TimeAgo), typeof(string), typeof(FeedCell), "");
        public static readonly BindableProperty ScoreProperty = BindableProperty.Create(nameof(Score), typeof(int), typeof(FeedCell), 0);
        public static readonly BindableProperty NumCommentsProperty = BindableProperty.Create(nameof(NumComments), typeof(int), typeof(FeedCell), 0);

        public int ItemNumber
        {
            get { return (int)GetValue(ItemNumberProperty); }
            set { SetValue(ItemNumberProperty, value); }
        }

        public string UrlText
        {
            get { return (string)GetValue(UrlTextProperty); }
            set { SetValue(UrlTextProperty, value); }
        }

        public string ArticleTitle
        {
            get { return (string)GetValue(ArticleTitleProperty); }
            set { SetValue(ArticleTitleProperty, value); }
        }

        public string SubmitterName
        {
            get { return (string)GetValue(SubmitterNameProperty); }
            set { SetValue(SubmitterNameProperty, value); }
        }

        public string TimeAgo
        {
            get { return (string)GetValue(TimeAgoProperty); }
            set { SetValue(TimeAgoProperty, value); }
        }

        public int Score
        {
            get { return (int)GetValue(ScoreProperty); }
            set { SetValue(ScoreProperty, value); }
        }

        public int NumComments
        {
            get { return (int)GetValue(NumCommentsProperty); }
            set { SetValue(NumCommentsProperty, value); }
        }

        public FeedCell()
        {
            var grid = new Grid
            {
                Padding = new Thickness(16, 0, 16, 16),
                ColumnSpacing = 16,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Auto, },
                    new ColumnDefinition { Width = GridLength.Star, },
                },
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto, },
                    new RowDefinition { Height = GridLength.Auto, },
                },
            };
            var separator = new ContentView
            {
                Padding = new Thickness(-grid.Padding.Left, 0, -grid.Padding.Right, 0),
                Content = new BoxView
                {
                    HeightRequest = 1,
                    Color = Colors.Black.WithAlpha(0.2f),
                    Margin = new Thickness(0, 0, 0, grid.Padding.Bottom - grid.RowSpacing),
                },
            };
            Grid.SetColumnSpan(separator, 2);
            // TODO(jpr): this breaks the cell sizing
            // grid.Add(separator);
            var f = new Frame
            {
                Padding = 0,
                HasShadow = false,

                IsClippedToBounds = true,
                CornerRadius = 6,

                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,

                Content = new Image
                {
                    BackgroundColor = Colors.Red.WithAlpha(0.3f),
                    WidthRequest = 70,
                    HeightRequest = 70,
                },
            };
            grid.Add(f, 0, 1);

            var sv = new VerticalStackLayout { Spacing = 8, };

            var idxLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 8, 0), };
            idxLbl.SetBinding(Label.TextProperty, new Binding(nameof(ItemNumber), BindingMode.OneWay, stringFormat: "{0}.", source: this));
            var urlLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
            urlLbl.SetBinding(Label.TextProperty, new Binding(nameof(UrlText), BindingMode.OneWay, source: this));
            sv.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    idxLbl,
                    new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Black.WithAlpha(0.1f), },
                    urlLbl,
                },
            });

            var titleLbl = new NoctalLabel { LineBreakMode = LineBreakMode.WordWrap, };
            titleLbl.SetBinding(Label.TextProperty, new Binding(nameof(ArticleTitle), BindingMode.OneWay, source: this));
            sv.Add(titleLbl);
            var submitterLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
            submitterLbl.SetBinding(Label.TextProperty, new Binding(nameof(SubmitterName), BindingMode.OneWay, source: this));
            var timeAgoLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
            timeAgoLbl.SetBinding(Label.TextProperty, new Binding(nameof(TimeAgo), BindingMode.OneWay, source: this));
            sv.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    submitterLbl,
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                    timeAgoLbl,
                },
            });

            var scoreLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
            scoreLbl.SetBinding(Label.TextProperty, new Binding(nameof(Score), BindingMode.OneWay, source: this));
            var commentsLbl = new NoctalLabel { VerticalOptions = LayoutOptions.Center, };
            commentsLbl.SetBinding(Label.TextProperty, new Binding(nameof(NumComments), BindingMode.OneWay, stringFormat: "{0} comments", source: this));
            sv.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Red.WithAlpha(0.1f), },
                    scoreLbl,
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                    commentsLbl,
                },
            });

            grid.Add(sv, 1, 1);

            Content = grid;
        }
    }
}
