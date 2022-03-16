namespace Noctal;

public class MainPage : ContentPage
{
    public MainPage()
    {
        var cv = new CollectionView();

        cv.ItemsSource = new[] {
            "A", "B", "C", "D", "E", "F", "G", "H",
            "A", "B", "C", "D", "E", "F", "G", "H",
            "A", "B", "C", "D", "E", "F", "G", "H",
        };
        cv.ItemTemplate = new DataTemplate(() => new FeedCell());
        var g = new Grid();
        g.Add(cv);
        Content = g;
    }

    private class FeedCell : ContentView
    {
        public FeedCell()
        {
            var g = new Grid
            {
                Padding = new Thickness(16, 0, 16, 16),
                ColumnSpacing = 16,
                RowSpacing = 8,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Auto, },
                    new ColumnDefinition { Width = GridLength.Star, },
                },
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto, },
                    new RowDefinition { Height = GridLength.Auto, },
                    new RowDefinition { Height = GridLength.Auto, },
                    new RowDefinition { Height = GridLength.Auto, },
                    new RowDefinition { Height = GridLength.Auto, },
                },
            };
            var separator = new ContentView
            {
                Padding = new Thickness(-g.Padding.Left, 0, -g.Padding.Right, 0),
                Content = new BoxView
                {
                    HeightRequest = 1,
                    Color = Colors.Black.WithAlpha(0.2f),
                    Margin = new Thickness(0, 0, 0, g.Padding.Bottom - g.RowSpacing),
                },
            };
            Grid.SetColumnSpan(separator, 2);
            g.Add(separator);
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
            Grid.SetRowSpan(f, 4);
            g.Add(f, 0, 1);
            g.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Black.WithAlpha(0.1f), },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "thinkcomposer.com", }
                },
            }, 1, 1);
            g.Add(new NoctalLabel
            {
                LineBreakMode = LineBreakMode.WordWrap,
                Text = "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models",
            }, 1, 2);
            g.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "Tomte", },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "18h ago", },
                },
            }, 1, 3);
            g.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Children = {
                    new BoxView { VerticalOptions = LayoutOptions.Center, HeightRequest = 16, WidthRequest = 16, Color = Colors.Red.WithAlpha(0.1f), },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "35", },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "•", },
                    new NoctalLabel { VerticalOptions = LayoutOptions.Center, Text = "9 comments", },
                },
            }, 1, 4);

            Content = g;
        }
    }
}
