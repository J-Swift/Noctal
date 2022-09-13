using ReactiveUI;
using System.Reactive.Disposables;

#if ANDROID
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using Google.Android.Material.Shape;
#elif IOS
using CoreFoundation;
using System.Runtime.InteropServices;
#endif

namespace Noctal.Stories;

public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    private int StoryId { get; set; }
    protected override StoryDetailViewModel CreateViewModel() => new(StoryId, new StoriesService());

    public static class Dims
    {
        public static readonly double DimImgFavicon = 16;
        public static readonly double DimImgHeightRatio = 0.6;
        public static readonly double DimImgRadius = 4;
        public static readonly double DimVPadding = 16;
        public static readonly double DimHPaddingRow = 4;
        public static readonly double DimHPadding = 20;
    }
}

#if ANDROID
public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    public const string NAVIGATION_ROUTE = "navigation_story";
    private const string ARGS_STORY_ID = "story_id";

    public static (int DestId, Bundle DestArgs) SafeNav(AndroidX.Navigation.NavController nav, int storyId)
    {
        var destId = nav.FindDestination(NAVIGATION_ROUTE).Id;
        var bundle = new Bundle();
        bundle.PutInt(ARGS_STORY_ID, storyId);
        return (destId, bundle);
    }

    private NoctalLabel LblTitle { get; set; } = null!;
    private NoctalLabel LblUrl { get; set; } = null!;
    private NoctalLabel LblScore { get; set; } = null!;
    private NoctalLabel LblAuthor { get; set; } = null!;
    private NoctalLabel LblTimeAgo { get; set; } = null!;

    protected override void ReadArgs(Bundle args)
    {
        StoryId = args.GetInt(ARGS_STORY_ID);
    }

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Url, v => v.LblUrl.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Score, v => v.LblScore.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Submitter, v => v.LblAuthor.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.TimeAgo, v => v.LblTimeAgo.Text)
            .DisposeWith(disposables);
    }

    protected override View CreateView(Context context)
    {
        var _dimImgFavicon = context.ToPixels(Dims.DimImgFavicon);
        var _dimImgRadius = context.ToPixels(Dims.DimImgRadius);
        var _dimVPadding = context.ToPixels(Dims.DimVPadding);
        var _dimHPaddingRow = context.ToPixels(Dims.DimHPaddingRow);
        var _dimHPadding = context.ToPixels(Dims.DimHPadding);

        var makeLabel = () => new NoctalLabel(context);
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
        sv.SetPaddingRelative(start: (int)_dimHPadding, end: (int)_dimHPadding, top: 0, bottom: 0);
        scroll.AddView(sv, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

        LblTitle = makeLabel();
        sv.AddView(LblTitle);

        // Url Row

        var row = makeRow();

        var shapeModel = new ShapeAppearanceModel().ToBuilder()
                .SetAllCornerSizes(new RelativeCornerSize(0.5f))
                .Build();
        var shape = new MaterialShapeDrawable(shapeModel);
        shape.FillColor = Colors.Red.WithAlpha(0.3f).ToDefaultColorStateList();

        var imgFavicon = new View(context)
        {
            Background = shape,
        };

        row.AddView(imgFavicon, new ViewGroup.LayoutParams((int)_dimImgFavicon, (int)_dimImgFavicon));

        addSpacer(row, _dimHPaddingRow);

        var lbl = makeLabel();
        LblUrl = lbl;
        row.AddView(lbl);

        addSpacer(sv, _dimVPadding);
        sv.AddView(row);

        // Image Row

        shapeModel = new ShapeAppearanceModel().ToBuilder()
           .SetAllCorners(CornerFamily.Rounded, _dimImgRadius)
           .Build();
        shape = new MaterialShapeDrawable(shapeModel)
        {
            FillColor = Colors.Red.WithAlpha(0.3f).ToDefaultColorStateList(),
        };

        var img = new View(context)
        {
            Background = shape,
        };

        addSpacer(sv, _dimVPadding);

        var imgWrapper = new ConstraintLayout(context);
        var parentId = ConstraintLayout.LayoutParams.ParentId;
        var ps = new ConstraintLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ConstraintLayout.LayoutParams.MatchConstraint)
        {
            DimensionRatio = $"1:{Dims.DimImgHeightRatio}",
            StartToStart = parentId,
            EndToEnd = parentId,
            TopToTop = parentId,
            BottomToBottom = parentId,
        };
        imgWrapper.AddView(img, ps);

        sv.AddView(imgWrapper, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

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

        addSpacer(sv, _dimVPadding);
        sv.AddView(row);

        return scroll;
    }
}
#elif IOS
public partial class StoryDetailPage : BasePage<StoryDetailViewModel>
{
    private NoctalLabel LblTitle { get; set; } = null!;
    private NoctalLabel LblUrl { get; set; } = null!;
    private NoctalLabel LblScore { get; set; } = null!;
    private NoctalLabel LblAuthor { get; set; } = null!;
    private NoctalLabel LblTimeAgo { get; set; } = null!;

    public StoryDetailPage(int storyId) : base()
    {
        StoryId = storyId;
    }

    protected override void BindView(CompositeDisposable disposables)
    {
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Title, v => v.LblTitle.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Url, v => v.LblUrl.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Score, v => v.LblScore.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.Submitter, v => v.LblAuthor.Text)
            .DisposeWith(disposables);
        this.OneWayBind(SafeViewModel, vm => vm.Item!.TimeAgo, v => v.LblTimeAgo.Text)
            .DisposeWith(disposables);
    }

    protected override UIView CreateView()
    {
        var makeLabel = () => new NoctalLabel { TranslatesAutoresizingMaskIntoConstraints = false, };
        var makeRow = () => new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow,
        };

        var scroll = new UIScrollView
        {
            BackgroundColor = SceneDelegate.Theme.BackgroundColor,
        };

        var sv = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = (nfloat)Dims.DimVPadding,
            Alignment = UIStackViewAlignment.Leading,
        };

        LblTitle = makeLabel();
        LblTitle.Lines = 0;
        sv.AddArrangedSubview(LblTitle);

        // Url Row

        var row = makeRow();

        var imgFavicon = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform(),
        };
        imgFavicon.WidthAnchor.ConstraintEqualTo((nfloat)Dims.DimImgFavicon).Active = true;
        imgFavicon.HeightAnchor.ConstraintEqualTo((nfloat)Dims.DimImgFavicon).Active = true;
        imgFavicon.Layer.CornerRadius = (nfloat)(Dims.DimImgFavicon / 2.0);
        row.AddArrangedSubview(imgFavicon);

        var lbl = makeLabel();
        LblUrl = lbl;
        row.AddArrangedSubview(lbl);

        sv.AddArrangedSubview(row);

        // Image Row

        var img = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform(),
        };
        img.Layer.CornerRadius = (nfloat)Dims.DimImgRadius;
        sv.AddArrangedSubview(img);
        img.WidthAnchor.ConstraintEqualTo(sv.WidthAnchor).Active = true;
        img.HeightAnchor.ConstraintEqualTo(img.WidthAnchor, (nfloat)Dims.DimImgHeightRatio).Active = true;

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

        sv.AddArrangedSubview(row); ;

        var svWrapper = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };
        svWrapper.AddSubview(sv);
        scroll.AddSubview(svWrapper);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            svWrapper.LeadingAnchor.ConstraintEqualTo(scroll.LeadingAnchor, (nfloat)Dims.DimHPadding),
            svWrapper.TrailingAnchor.ConstraintEqualTo(scroll.TrailingAnchor, -(nfloat)Dims.DimHPadding),
            svWrapper.TopAnchor.ConstraintEqualTo(scroll.TopAnchor),
            svWrapper.BottomAnchor.ConstraintEqualTo(scroll.BottomAnchor),

            svWrapper.WidthAnchor.ConstraintEqualTo(scroll.WidthAnchor, 1, -2*(nfloat)Dims.DimHPadding),

            sv.LeadingAnchor.ConstraintEqualTo(svWrapper.LeadingAnchor),
            sv.TrailingAnchor.ConstraintEqualTo(svWrapper.TrailingAnchor),
            sv.TopAnchor.ConstraintEqualTo(svWrapper.TopAnchor),
            sv.BottomAnchor.ConstraintEqualTo(svWrapper.BottomAnchor),
        });

        return scroll;
    }
}
#endif
