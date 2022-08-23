using Noctal.Stories.Models;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

#if ANDROID
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
#elif IOS
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using UIKit;
#endif

namespace Noctal.Stories;

public partial class StoriesPage : BasePage<StoriesViewModel>
{
    public record EventArgs(StoriesFeedItem SelectedItem);
    public EventHandler<EventArgs>? OnItemSelected;

    protected override StoriesViewModel CreateViewModel() => new(new StoriesService());

    public static class Dims
    {
        public static readonly double DimImg = 70;
        public static readonly double DimImgRadius = 4;
        public static readonly double DimVPadding = 16;
        public static readonly double DimHPaddingRow = 4;
        public static readonly double DimHPadding = 20;

        public static readonly double DimEstimatedCellHeight = 160;
    }
}

#if ANDROID
public partial class StoriesPage
{
    private RecyclerView Feed { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
    }

    protected override View CreateView(Context ctx)
    {
        var layoutManager = new LinearLayoutManager(ctx, LinearLayoutManager.Vertical, false);
        var adapter = new MyAdapter(SafeViewModel.Items);
        var decor = new DividerItemDecoration(ctx, DividerItemDecoration.Vertical);

        Feed = new RecyclerView(ctx);
        Feed.SetLayoutManager(layoutManager);
        Feed.SetAdapter(adapter);
        Feed.AddItemDecoration(decor);
        adapter.ItemSelected += Adapter_OnItemSelected;
        return Feed;
    }

    private void Adapter_OnItemSelected(object? sender, MyAdapter.EventArgs e)
    {
        OnItemSelected?.Invoke(this, new EventArgs(e.SelectedItem));
    }
}

class MyAdapter : RecyclerView.Adapter
{
    private readonly ReadOnlyObservableCollection<StoriesFeedItem> Items;
    public record EventArgs(StoriesFeedItem SelectedItem);
    public EventHandler<EventArgs>? ItemSelected;

    public MyAdapter(ReadOnlyObservableCollection<StoriesFeedItem> items) : base()
    {
        Items = items;
    }

    public override int ItemCount => Items.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = CreateView(parent.Context!);
        return new ViewHolder(view);
    }

    private View CreateView(Android.Content.Context context)
    {
        var dimImg = context.ToPixels(StoriesPage.Dims.DimImg);
        var dimVPadding = context.ToPixels(StoriesPage.Dims.DimVPadding);
        var dimHPadding = context.ToPixels(StoriesPage.Dims.DimHPadding);
        var dimSpacerPs = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)dimVPadding);
        var parent = ConstraintLayout.LayoutParams.ParentId;

        var container = new ConstraintLayout(context);

        var img = new View(context);
        img.SetBackgroundColor(Colors.Red.WithAlpha(0.3f).ToPlatform());
        img.Id = 282971;
        container.AddView(img);

        var sv = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
        };
        sv.Id = 18310;
        container.AddView(sv);

        var spacer = new View(context);
        sv.AddView(spacer, dimSpacerPs);

        var tv = new TextView(context);
        tv.Text = "26. thinkcomposer.com";
        sv.AddView(tv, new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent));

        spacer = new View(context);
        sv.AddView(spacer, dimSpacerPs);

        tv = new TextView(context);
        tv.Id = Android.Resource.Id.Text1;
        sv.AddView(tv, new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent));

        spacer = new View(context);
        sv.AddView(spacer, dimSpacerPs);

        tv = new TextView(context);
        tv.Text = "Tomte * 18h ago";
        sv.AddView(tv, new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent));

        spacer = new View(context);
        sv.AddView(spacer, dimSpacerPs);

        tv = new TextView(context);
        tv.Text = "^35 * 9 comments";
        sv.AddView(tv, new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent));

        spacer = new View(context);
        sv.AddView(spacer, dimSpacerPs);

        var ps = new ConstraintLayout.LayoutParams((int)dimImg, (int)dimImg);
        //ps.ConstrainedWidth = true;
        //ps.ConstrainedHeight = true;
        ps.LeftToLeft = parent;
        ps.RightToLeft = sv.Id;
        ps.TopToTop = parent;
        ps.MarginStart = (int)dimHPadding;
        ps.BottomToBottom = parent;
        img.LayoutParameters = ps;

        ps = new ConstraintLayout.LayoutParams(ConstraintLayout.LayoutParams.MatchConstraint, ConstraintLayout.LayoutParams.WrapContent);
        ps.ConstrainedHeight = true;
        ps.LeftToRight = img.Id;
        ps.RightToRight = parent;
        ps.TopToTop = parent;
        ps.BottomToBottom = parent;
        ps.SetMargins(top: 0, bottom: 0, left: (int)dimHPadding, right: (int)dimHPadding);
        sv.LayoutParameters = ps;

        container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        return container;
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var tHolder = (ViewHolder)holder;
        var item = Items[position];

        tHolder.Bind(item, () => ItemSelected?.Invoke(this, new EventArgs(item)));
    }

    private class ViewHolder : RecyclerView.ViewHolder
    {
        private readonly View Container;
        private readonly TextView LblText;
        private Action? OnClick;

        public ViewHolder(View view) : base(view)
        {
            Container = view;
            LblText = view.FindViewById<TextView>(Android.Resource.Id.Text1)!;
        }

        public void Bind(StoriesFeedItem model, Action onClick)
        {
            LblText.Text = model.Title;
            OnClick = onClick;
            Container.Click -= HandleClick;
            Container.Click += HandleClick;
        }

        private void HandleClick(object? sender, System.EventArgs e)
        {
            OnClick?.Invoke();
        }
    }
}
#elif IOS
public partial class StoriesPage : IUICollectionViewDelegate
{
    private UICollectionView Feed { get; set; } = null!;
    private UICollectionViewDiffableDataSource<NSNumber, NSNumber> DataSource { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables) { }

    protected override UIView CreateView()
    {
        var view = new UIView();

        var itemSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated((NFloat)Dims.DimEstimatedCellHeight)
        );
        var item = NSCollectionLayoutItem.Create(itemSize);

        var groupSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated((NFloat)Dims.DimEstimatedCellHeight)
        );
        var group = NSCollectionLayoutGroup.CreateHorizontal(groupSize, item);

        var section = NSCollectionLayoutSection.Create(group);

        var layout = new UICollectionViewCompositionalLayout(section);

        Feed = new UICollectionView(CGRect.Empty, layout)
        {
            Delegate = this,
        };

        var cellReg = UICollectionViewCellRegistration.GetRegistration(typeof(UICollectionViewCell), (cell, indexPath, item) =>
        {
            var tItem = ((ObjectWrapper<StoriesFeedItem>)item).Value;
            var contentConfig = new StoryFeedView.StoryFeedConfiguration
            {
                ItemNumber = indexPath.Row + 1,
                Url = tItem.Url,
                Title = tItem.Title,
                Submitter = tItem.Submitter,
                TimeAgo = tItem.TimeAgo,
                Score = tItem.Score,
                NumComments = tItem.NumComments,
            };
            cell.ContentConfiguration = contentConfig;

            var bgConfig = UIBackgroundConfiguration.ListPlainCellConfiguration;
            cell.BackgroundConfiguration = bgConfig;
        });

        DataSource = new UICollectionViewDiffableDataSource<NSNumber, NSNumber>(Feed, (collectionView, indexPath, itemIdentifier) =>
        {
            var item = SafeViewModel.GetItem(((NSNumber)itemIdentifier).Int32Value);
            return collectionView.DequeueConfiguredReusableCell(cellReg, indexPath, new ObjectWrapper<StoriesFeedItem>(item));
        });

        Feed.RegisterClassForCell(typeof(UICollectionViewCell), "cell");

        return Feed;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var snapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber>();
        snapshot.AppendSections(new NSNumber[] { 1 });
        snapshot.AppendItems(SafeViewModel.Items.Select(it => (NSNumber)it.Id).ToArray(), 1);
        DataSource.ApplySnapshot(snapshot, true);
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        foreach (var item in Feed.GetIndexPathsForSelectedItems())
        {
            Feed.DeselectItem(item, true);
        }
    }

    [Export("collectionView:didSelectItemAtIndexPath:")]
    public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var item = SafeViewModel.Items[indexPath.Row]!;
        OnItemSelected?.Invoke(this, new EventArgs(item!));
    }
}

//public record StoriesFeedItem(int Id, string Url, string Title, string Submitter, string TimeAgo, int Score, int NumComments)
class StoryFeedView : UIView, IUIContentView
{
    public class StoryFeedConfiguration : NSObject, IUIContentConfiguration
    {
        public int ItemNumber { get; set; }
        public string Url { get; set; } = "";
        public string Title { get; set; } = "";
        public string Submitter { get; set; } = "";
        public string TimeAgo { get; set; } = "";
        public int Score { get; set; }
        public int NumComments { get; set; }

        [return: Release]
        public NSObject Copy(NSZone? zone)
        {
            return new StoryFeedConfiguration
            {
                ItemNumber = ItemNumber,
                Url = Url,
                Title = Title,
                Submitter = Submitter,
                TimeAgo = TimeAgo,
                Score = Score,
                NumComments = NumComments,
            };
        }

        public IUIContentConfiguration GetUpdatedConfiguration(IUIConfigurationState state) => this;

        public IUIContentView MakeContentView()
        {
            return new StoryFeedView(this);
        }
    }

    private IUIContentConfiguration _configuration;
    public IUIContentConfiguration Configuration
    {
        get => _configuration;
        set
        {
            _configuration = value;
            updateConfiguration(value);
        }
    }

    private readonly NoctalLabel LblArticleNumber;
    private readonly NoctalLabel LblUrl;
    private readonly NoctalLabel LblTitle;
    private readonly NoctalLabel LblAuthor;
    private readonly NoctalLabel LblTimeAgo;
    private readonly NoctalLabel LblScore;
    private readonly NoctalLabel LblNumComments;

    private StoryFeedView(StoryFeedConfiguration configuration) : base(CGRect.Empty)
    {
        _configuration = configuration;

        var makeLabel = () => new NoctalLabel { TranslatesAutoresizingMaskIntoConstraints = false, };

        var img = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform(),
        };
        img.Layer.CornerRadius = (nfloat)StoriesPage.Dims.DimImgRadius;
        AddSubview(img);

        var sv = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Vertical,
            Alignment = UIStackViewAlignment.Leading,
            Spacing = (nfloat)StoriesPage.Dims.DimVPadding,
        };
        AddSubview(sv);

        // Url Row

        var row = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow,
        };

        var lbl = makeLabel();
        LblArticleNumber = lbl;
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        LblUrl = lbl;
        row.AddArrangedSubview(lbl);

        sv.AddArrangedSubview(row);

        // Title Row

        lbl = makeLabel();
        lbl.Lines = 0;
        LblTitle = lbl;
        sv.AddArrangedSubview(lbl);

        // Author Row

        row = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow,
        };

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

        // Score Row

        row = new UIStackView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow,
        };

        lbl = makeLabel();
        LblScore = lbl;
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        lbl.Text = "•";
        row.AddArrangedSubview(lbl);

        lbl = makeLabel();
        LblNumComments = lbl;
        row.AddArrangedSubview(lbl);

        sv.AddArrangedSubview(row);

        var separator = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.OpaqueSeparator,
        };
        AddSubview(separator);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            separator.HeightAnchor.ConstraintEqualTo(1),
            separator.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            separator.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
            separator.BottomAnchor.ConstraintEqualTo(BottomAnchor),

            img.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, (nfloat)StoriesPage.Dims.DimHPadding),
            img.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
            img.WidthAnchor.ConstraintEqualTo((nfloat)StoriesPage.Dims.DimImg),
            img.HeightAnchor.ConstraintEqualTo(img.WidthAnchor),

            sv.LeadingAnchor.ConstraintEqualTo(img.TrailingAnchor, (nfloat)StoriesPage.Dims.DimHPadding),
            sv.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -(nfloat)StoriesPage.Dims.DimHPadding),
            sv.TopAnchor.ConstraintEqualTo(TopAnchor, (nfloat)StoriesPage.Dims.DimVPadding),
            sv.BottomAnchor.ConstraintEqualTo(separator.TopAnchor, -(nfloat)StoriesPage.Dims.DimVPadding),
        });

        updateConfiguration(configuration);
    }

    private void updateConfiguration(IUIContentConfiguration weakConfig)
    {
        if (weakConfig is StoryFeedConfiguration config)
        {
            LblArticleNumber.Text = $"{config.ItemNumber}.";
            LblUrl.Text = config.Url;
            LblTitle.Text = config.Title;
            LblAuthor.Text = config.Submitter;
            LblTimeAgo.Text = config.TimeAgo;
            LblScore.Text = config.Score.ToString();
            LblNumComments.Text = $"{config.NumComments} comment" + (config.NumComments > 1 ? "s" : "");
        }
    }
}

class ObjectWrapper<T> : NSObject where T : class
{
    public T Value { get; }

    public ObjectWrapper(T value) : base()
    {
        Value = value;
    }
}
#endif
