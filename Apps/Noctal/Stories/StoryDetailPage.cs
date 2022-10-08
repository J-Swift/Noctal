using Noctal.ImageLoading;
using Noctal.UI.Theming;
using ReactiveUI;
using System.Reactive.Disposables;

#if ANDROID
using AndroidX.AppCompat.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Navigation;
using Google.Android.Material.Shape;
using Google.Android.Material.ImageView;

#elif IOS
using Foundation;
#endif

namespace Noctal.Stories;

public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private int StoryId { get; set; }

    protected override StoryDetailViewModel CreateViewModel()
    {
        return new StoryDetailViewModel(StoryId, ServiceProvider.GetService<StoriesService>());
    }

    internal static class Dims
    {
        public static readonly double DimImgFavicon = 16;
        public static readonly double DimImgHeightRatio = 0.6;
        public static readonly double DimImgRadius = 4;

        public static readonly double DimVPadding = 16;

        public static readonly double DimHPaddingRow = 4;
        public static readonly double DimHPadding = 20;
    }

    internal static class Styling
    {
        public static readonly double FontSizeDefault = 19;
        public static readonly double FontSizeTitle = 21;
    }
}

#if ANDROID
public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    public const string NAVIGATION_ROUTE = "navigation_story";
    private const string ARGS_STORY_ID = "story_id";

    private NoctalLabel LblTitle { get; set; } = null!;
    private NoctalLabel LblUrl { get; set; } = null!;
    private NoctalLabel LblScore { get; set; } = null!;
    private NoctalLabel LblAuthor { get; set; } = null!;
    private NoctalLabel LblTimeAgo { get; set; } = null!;
    private ImageView ImgFavicon { get; set; } = null!;
    private ImageView ImgImage { get; set; } = null!;
    private ViewGroup ImgImageWrapper { get; set; } = null!;
    private NoctalLabel LblBody { get; set; } = null!;

    public static (int DestId, Bundle DestArgs) SafeNav(NavController nav, int storyId)
    {
        var destId = nav.FindDestination(NAVIGATION_ROUTE).Id;
        var bundle = new Bundle();
        bundle.PutInt(ARGS_STORY_ID, storyId);
        return (destId, bundle);
    }

    protected override void ReadArgs(Bundle args)
    {
        StoryId = args.GetInt(ARGS_STORY_ID);
    }

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.DisplayUrl, v => v.LblUrl.Text, it => it ?? " ")
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Score, v => v.LblScore.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Submitter, v => v.LblAuthor.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.TimeAgo, v => v.LblTimeAgo.Text)
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.StoryText)
            .Subscribe(it =>
            {
                LblBody.Visibility = it == null ? ViewStates.Gone : ViewStates.Visible;

                if (it == null)
                {
                    return;
                }

                LblBody.Text = Html.FromHtml(it, FromHtmlOptions.ModeLegacy)!.ToString();
            })
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.FavIconPath)
            .Subscribe(it =>
            {
                ImgFavicon.Visibility = it == null ? ViewStates.Gone : ViewStates.Visible;
                var svc = ServiceProvider.GetService<IImageLoader>();
                svc.LoadInto(this, new IImageLoader.LoadRequest(ImgFavicon, it));
            })
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.ImagePath)
            .Subscribe(it =>
            {
                ImgImageWrapper.Visibility = it == null ? ViewStates.Gone : ViewStates.Visible;
                var svc = ServiceProvider.GetService<IImageLoader>();
                svc.LoadInto(this, new IImageLoader.LoadRequest(ImgImage, it));
            })
            .DisposeWith(disposables);
    }

    protected override View CreateView(Context context)
    {
        var _dimImgFavicon = context.ToPixels(Dims.DimImgFavicon);
        var _dimImgRadius = context.ToPixels(Dims.DimImgRadius);
        var _dimVPadding = context.ToPixels(Dims.DimVPadding);
        var _dimHPaddingRow = context.ToPixels(Dims.DimHPaddingRow);
        var _dimHPadding = context.ToPixels(Dims.DimHPadding);

        var isNight = AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightYes;

        var makeLabel = () => new NoctalLabel(context)
        {
            TextSize = (float)Styling.FontSizeDefault,
        };
        var makeRow = () => new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
        };
        var addSpacer = (LinearLayout layout, float space) =>
        {
            var view = new View(context);
            var ps = new ViewGroup.LayoutParams(1, 1);
            if (layout.Orientation == Orientation.Horizontal)
            {
                ps.Width = (int)space;
            }
            else
            {
                ps.Height = (int)space;
            }

            layout.AddView(view, ps);
        };

        var scroll = new ScrollView(context);

        var sv = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
        };
        sv.SetPaddingRelative((int)_dimHPadding, end: (int)_dimHPadding, top: 0, bottom: 0);
        scroll.AddView(sv, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

        LblTitle = makeLabel();
        LblTitle.TextSize = (float)Styling.FontSizeTitle;
        sv.AddView(LblTitle);

        // Url Row

        var row = makeRow();
        row.SetGravity(GravityFlags.CenterVertical);

        var shapeModel = new ShapeAppearanceModel().ToBuilder()
            .SetAllCornerSizes(new RelativeCornerSize(0.5f))
            .Build();

        ImgFavicon = new ShapeableImageView(context)
        {
            ShapeAppearanceModel = shapeModel,
        };

        row.AddView(ImgFavicon, new ViewGroup.LayoutParams((int)_dimImgFavicon, (int)_dimImgFavicon));
        ((LinearLayout.LayoutParams)ImgFavicon.LayoutParameters!).MarginEnd = (int)_dimHPaddingRow;

        var lbl = makeLabel();
        lbl.SetTextColor((isNight ? new DarkTheme() as ITheme : new LightTheme() as ITheme).PrimaryColor.ToPlatform());
        LblUrl = lbl;
        row.AddView(lbl);

        addSpacer(sv, _dimVPadding);
        sv.AddView(row);

        // Image Row

        shapeModel = new ShapeAppearanceModel().ToBuilder()
            .SetAllCorners(CornerFamily.Rounded, _dimImgRadius)
            .Build();

        ImgImage = new ShapeableImageView(context)
        {
            ShapeAppearanceModel = shapeModel,
        };
        ImgImage.SetScaleType(ImageView.ScaleType.CenterCrop);

        addSpacer(sv, _dimVPadding);

        ImgImageWrapper = new ConstraintLayout(context);
        var parentId = ConstraintLayout.LayoutParams.ParentId;
        var ps = new ConstraintLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ConstraintLayout.LayoutParams.MatchConstraint)
        {
            DimensionRatio = $"1:{Dims.DimImgHeightRatio}",
            StartToStart = parentId,
            EndToEnd = parentId,
            TopToTop = parentId,
            BottomToBottom = parentId,
        };
        ImgImageWrapper.AddView(ImgImage, ps);

        sv.AddView(ImgImageWrapper, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        ((LinearLayout.LayoutParams)ImgImageWrapper.LayoutParameters!).BottomMargin = (int)_dimVPadding;

        // Body Row

        LblBody = makeLabel();
        sv.AddView(LblBody);
        ((LinearLayout.LayoutParams)LblBody.LayoutParameters!).BottomMargin = (int)_dimVPadding;

        // Author Row

        row = makeRow();

        lbl = makeLabel();
        LblScore = lbl;
        row.AddView(lbl);

        addSpacer(row, _dimHPaddingRow);

        lbl = makeLabel();
        lbl.Text = "•";
        row.AddView(lbl);

        addSpacer(row, _dimHPaddingRow);

        lbl = makeLabel();
        LblAuthor = lbl;
        row.AddView(lbl);

        addSpacer(row, _dimHPaddingRow);

        lbl = makeLabel();
        lbl.Text = "•";
        row.AddView(lbl);

        addSpacer(row, _dimHPaddingRow);

        lbl = makeLabel();
        LblTimeAgo = lbl;
        row.AddView(lbl);

        sv.AddView(row);

        return scroll;
    }
}
#elif IOS
public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    public StoryDetailPage(int storyId)
    {
        StoryId = storyId;
    }

    private NoctalLabel LblTitle { get; set; } = null!;
    private NoctalLabel LblUrl { get; set; } = null!;
    private NoctalLabel LblScore { get; set; } = null!;
    private NoctalLabel LblAuthor { get; set; } = null!;
    private NoctalLabel LblTimeAgo { get; set; } = null!;
    private UIImageView ImgFavicon { get; set; } = null!;
    private UIImageView ImgImage { get; set; } = null!;
    private NoctalLabel LblBody { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.DisplayUrl, v => v.LblUrl.Text, it => it ?? " ")
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Score, v => v.LblScore.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Submitter, v => v.LblAuthor.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.TimeAgo, v => v.LblTimeAgo.Text)
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.StoryText)
            .Subscribe(it =>
            {
                LblBody.Hidden = it == null;

                if (it == null)
                {
                    return;
                }

                // Cache these so we can use them again after setting attributed string
                // since iOS removes them...
                var font = LblBody.Font;
                var textColor = LblBody.TextColor;

                var data = new NSString(it).Encode(NSStringEncoding.UTF8);
                var options = new NSAttributedStringDocumentAttributes
                {
                    DocumentType = NSDocumentType.HTML, StringEncoding = NSStringEncoding.UTF8,
                };
                NSError e = null!;
                LblBody.AttributedText = new NSAttributedString(data, options.Dictionary, out _, ref e);

                LblBody.Font = font;
                LblBody.TextColor = textColor;
            })
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.FavIconPath)
            .Subscribe(it =>
            {
                ImgFavicon.Hidden = it == null;
                var svc = ServiceProvider.GetService<IImageLoader>();
                svc.LoadInto(new IImageLoader.LoadRequest(ImgFavicon, it));
            })
            .DisposeWith(disposables);
        SafeViewModel.WhenAnyValue(vm => vm.Item!.ImagePath)
            .Subscribe(it =>
            {
                ImgImage.Hidden = it == null;
                var svc = ServiceProvider.GetService<IImageLoader>();
                svc.LoadInto(new IImageLoader.LoadRequest(ImgImage, it));
            })
            .DisposeWith(disposables);
    }

    protected override UIView CreateView()
    {
        var makeLabel = () =>
        {
            var lbl = new NoctalLabel { TranslatesAutoresizingMaskIntoConstraints = false, };
            lbl.Font = lbl.Font.WithSize((nfloat)Styling.FontSizeDefault);
            return lbl;
        };
        var makeRow = () => new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = (nfloat)Dims.DimHPaddingRow,
        };

        var scroll = new UIScrollView { BackgroundColor = SceneDelegate.Theme.BackgroundColor, };

        var sv = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = (nfloat)Dims.DimVPadding,
            Alignment = UIStackViewAlignment.Leading,
        };

        LblTitle = makeLabel();
        LblTitle.Font = LblTitle.Font.WithSize((nfloat)Styling.FontSizeTitle);
        LblTitle.Lines = 0;
        sv.AddArrangedSubview(LblTitle);

        // Url Row

        var row = makeRow();
        row.Alignment = UIStackViewAlignment.Center;

        ImgFavicon = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill,
        };
        ImgFavicon.WidthAnchor.ConstraintEqualTo((nfloat)Dims.DimImgFavicon).Active = true;
        ImgFavicon.HeightAnchor.ConstraintEqualTo((nfloat)Dims.DimImgFavicon).Active = true;
        ImgFavicon.Layer.CornerRadius = (nfloat)(Dims.DimImgFavicon / 2.0);
        row.AddArrangedSubview(ImgFavicon);

        var lbl = makeLabel();
        LblUrl = lbl;
        lbl.TextColor = SceneDelegate.Theme.PrimaryColor;
        row.AddArrangedSubview(lbl);

        sv.AddArrangedSubview(row);

        // Image Row

        ImgImage = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill,
        };
        ImgImage.Layer.CornerRadius = (nfloat)Dims.DimImgRadius;
        sv.AddArrangedSubview(ImgImage);
        ImgImage.WidthAnchor.ConstraintEqualTo(sv.WidthAnchor).Active = true;
        ImgImage.HeightAnchor.ConstraintEqualTo(ImgImage.WidthAnchor, (nfloat)Dims.DimImgHeightRatio).Active = true;

        // Body Row

        LblBody = makeLabel();
        LblBody.Lines = 0;
        sv.AddArrangedSubview(LblBody);

        // Author Row

        row = makeRow();

        lbl = makeLabel();
        LblScore = lbl;
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        lbl.Text = "•";
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        LblAuthor = lbl;
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        lbl.Text = "•";
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        LblTimeAgo = lbl;
        row.AddArrangedSubview(lbl);

        sv.AddArrangedSubview(row);

        var svWrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, };
        svWrapper.AddSubview(sv);
        scroll.AddSubview(svWrapper);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            svWrapper.LeadingAnchor.ConstraintEqualTo(scroll.LeadingAnchor, (nfloat)Dims.DimHPadding),
            svWrapper.TrailingAnchor.ConstraintEqualTo(scroll.TrailingAnchor, -(nfloat)Dims.DimHPadding),
            svWrapper.TopAnchor.ConstraintEqualTo(scroll.TopAnchor),
            svWrapper.BottomAnchor.ConstraintEqualTo(scroll.BottomAnchor),

            svWrapper.WidthAnchor.ConstraintEqualTo(scroll.WidthAnchor, 1, -2 * (nfloat)Dims.DimHPadding),

            sv.LeadingAnchor.ConstraintEqualTo(svWrapper.LeadingAnchor),
            sv.TrailingAnchor.ConstraintEqualTo(svWrapper.TrailingAnchor),
            sv.TopAnchor.ConstraintEqualTo(svWrapper.TopAnchor),
            sv.BottomAnchor.ConstraintEqualTo(svWrapper.BottomAnchor),
        });

        return scroll;
    }
}
#endif
