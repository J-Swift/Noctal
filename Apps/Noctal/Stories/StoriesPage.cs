using DynamicData.Binding;
using Noctal.Stories.Models;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#if ANDROID
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Shape;

#elif IOS
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
#endif

namespace Noctal.Stories;

public partial class StoriesPage : BasePage<StoriesViewModel>
{
    public event EventHandler<EventArgs>? OnItemSelected;

    protected override StoriesViewModel CreateViewModel()
    {
        return new StoriesViewModel(ServiceProvider.GetService<StoriesService>());
    }

    public record EventArgs(StoriesFeedItem SelectedItem);

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
    public const string NAVIGATION_ROUTE = "navigation_stories";

    private RecyclerView Feed { get; set; } = null!;
    private MyAdapter ItemAdapter { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
        SafeViewModel.Items
            .ObserveCollectionChanges()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                ItemAdapter.SetItems(new List<StoriesFeedItem>(SafeViewModel.Items));
            })
            .DisposeWith(disposables);
    }

    protected override View CreateView(Context ctx)
    {
        var layoutManager = new LinearLayoutManager(ctx, LinearLayoutManager.Vertical, false);
        ItemAdapter = new MyAdapter(new List<StoriesFeedItem>(SafeViewModel.Items));
        var decor = new DividerItemDecoration(ctx, DividerItemDecoration.Vertical);

        Feed = new RecyclerView(ctx);
        Feed.SetLayoutManager(layoutManager);
        Feed.SetAdapter(ItemAdapter);
        Feed.AddItemDecoration(decor);
        ItemAdapter.ItemSelected += Adapter_OnItemSelected;
        return Feed;
    }

    private void Adapter_OnItemSelected(object? sender, MyAdapter.EventArgs e)
    {
        OnItemSelected?.Invoke(this, new EventArgs(e.SelectedItem));
    }
}

internal class MyAdapter : RecyclerView.Adapter
{
    private static readonly int LblArticleNumberId = 839183;
    private static readonly int LblUrlId = 18809001;
    private static readonly int LblTitleId = 859891;
    private static readonly int LblAuthorId = 585981;
    private static readonly int LblTimeAgoId = 38981;
    private static readonly int LblScoreId = 1828984;
    private static readonly int LblNumCommentsId = 284981;
    private IList<StoriesFeedItem> Items;
    public EventHandler<EventArgs>? ItemSelected;

    public MyAdapter(IList<StoriesFeedItem> items)
    {
        Items = items;
    }

    public override int ItemCount => Items.Count;

    public void SetItems(IList<StoriesFeedItem> newItems)
    {
        Console.WriteLine("JIMMY SetItems");
        var cb = new Callback(Items, newItems);
        var diff = DiffUtil.CalculateDiff(cb);
        Items = newItems;
        diff.DispatchUpdatesTo(this);
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = CreateView(parent.Context!);
        return new ViewHolder(view);
    }

    private View CreateView(Context context)
    {
        var dimImg = context.ToPixels(StoriesPage.Dims.DimImg);
        var dimImgRadius = context.ToPixels(StoriesPage.Dims.DimImgRadius);
        var dimVPadding = context.ToPixels(StoriesPage.Dims.DimVPadding);
        var dimHPadding = context.ToPixels(StoriesPage.Dims.DimHPadding);
        var dimHPaddingRow = context.ToPixels(StoriesPage.Dims.DimHPaddingRow);
        var dimVSpacerPs = new LinearLayout.LayoutParams(1, (int)dimVPadding);
        var dimHSpacerPs = new LinearLayout.LayoutParams((int)dimHPaddingRow, 1);
        var parent = ConstraintLayout.LayoutParams.ParentId;

        var container = new ConstraintLayout(context);

        var shapeModel = new ShapeAppearanceModel().ToBuilder()
            .SetAllCorners(CornerFamily.Rounded, dimImgRadius)
            .Build();
        var shape = new MaterialShapeDrawable(shapeModel);
        shape.FillColor = Colors.Red.WithAlpha(0.3f).ToDefaultColorStateList();

        var img = new View(context) { Id = 282971, Background = shape };

        container.AddView(img);

        var sv = new LinearLayout(context) { Orientation = Orientation.Vertical, Id = 18310 };
        container.AddView(sv);

        // Url Row

        var row = new LinearLayout(context) { Orientation = Orientation.Horizontal };

        var tv = new NoctalLabel(context) { Id = LblArticleNumberId };
        row.AddView(tv);

        var spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context) { Id = LblUrlId };
        row.AddView(tv);

        sv.AddView(row);

        // Title Row

        tv = new NoctalLabel(context) { Id = LblTitleId };

        spacer = new View(context);
        sv.AddView(spacer, dimVSpacerPs);
        sv.AddView(tv);

        // Author Row

        row = new LinearLayout(context) { Orientation = Orientation.Horizontal };

        tv = new NoctalLabel(context) { Id = LblAuthorId };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context) { Text = "•" };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context) { Id = LblTimeAgoId, Text = "•" };
        row.AddView(tv);

        spacer = new View(context);
        sv.AddView(spacer, dimVSpacerPs);
        sv.AddView(row);

        // Score Row

        row = new LinearLayout(context) { Orientation = Orientation.Horizontal };

        tv = new NoctalLabel(context) { Id = LblScoreId };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context) { Text = "•" };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context) { Id = LblNumCommentsId, Text = "•" };
        row.AddView(tv);

        spacer = new View(context);
        sv.AddView(spacer, dimVSpacerPs);
        sv.AddView(row);

        var ps = new ConstraintLayout.LayoutParams((int)dimImg, (int)dimImg);
        ps.LeftToLeft = parent;
        ps.RightToLeft = sv.Id;
        ps.TopToTop = parent;
        ps.MarginStart = (int)dimHPadding;
        ps.BottomToBottom = parent;
        img.LayoutParameters = ps;

        ps = new ConstraintLayout.LayoutParams(ConstraintLayout.LayoutParams.MatchConstraint, ViewGroup.LayoutParams.WrapContent);
        ps.ConstrainedHeight = true;
        ps.LeftToRight = img.Id;
        ps.RightToRight = parent;
        ps.TopToTop = parent;
        ps.BottomToBottom = parent;
        ps.SetMargins(top: (int)dimVPadding, bottom: (int)dimVPadding, left: (int)dimHPadding, right: (int)dimHPadding);
        sv.LayoutParameters = ps;

        container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        return container;
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var tHolder = (ViewHolder)holder;
        var item = Items[position];

        tHolder.Bind(position + 1, item, () => ItemSelected?.Invoke(this, new EventArgs(item)));
    }

    public record EventArgs(StoriesFeedItem SelectedItem);

    private class Callback : DiffUtil.Callback
    {
        private readonly IList<StoriesFeedItem> NewList;
        private readonly IList<StoriesFeedItem> OldList;

        public Callback(IList<StoriesFeedItem> oldList, IList<StoriesFeedItem> newList)
        {
            OldList = oldList;
            NewList = newList;
        }

        public override int OldListSize => OldList.Count;
        public override int NewListSize => NewList.Count;

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            var oldItem = OldList[oldItemPosition];
            var newItem = NewList[newItemPosition];

            return oldItem.Id == newItem.Id;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            var oldItem = OldList[oldItemPosition];
            var newItem = NewList[newItemPosition];

            return oldItem.Equals(newItem);
        }
    }

    private class ViewHolder : RecyclerView.ViewHolder
    {
        private readonly View Container;
        private readonly NoctalLabel LblArticleNumber;
        private readonly NoctalLabel LblAuthor;
        private readonly NoctalLabel LblNumComments;
        private readonly NoctalLabel LblScore;
        private readonly NoctalLabel LblTimeAgo;
        private readonly NoctalLabel LblTitle;
        private readonly NoctalLabel LblUrl;
        private Action? OnClick;

        public ViewHolder(View view) : base(view)
        {
            Container = view;
            LblArticleNumber = view.FindViewById<NoctalLabel>(LblArticleNumberId)!;
            LblUrl = view.FindViewById<NoctalLabel>(LblUrlId)!;
            LblTitle = view.FindViewById<NoctalLabel>(LblTitleId)!;
            LblAuthor = view.FindViewById<NoctalLabel>(LblAuthorId)!;
            LblTimeAgo = view.FindViewById<NoctalLabel>(LblTimeAgoId)!;
            LblScore = view.FindViewById<NoctalLabel>(LblScoreId)!;
            LblNumComments = view.FindViewById<NoctalLabel>(LblNumCommentsId)!;
        }

        public void Bind(int articleNumber, StoriesFeedItem model, Action onClick)
        {
            LblArticleNumber.Text = $"{articleNumber}.";
            LblUrl.Text = model.Url;
            LblTitle.Text = model.Title;
            LblAuthor.Text = model.Submitter;
            LblTimeAgo.Text = model.TimeAgo;
            LblScore.Text = model.Score.ToString();
            LblNumComments.Text = $"{model.NumComments} comment" + (model.NumComments > 1 ? "s" : "");

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

    protected override void BindView(CompositeDisposable disposables)
    {
        SafeViewModel.Items
            .ObserveCollectionChanges()
            .ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
            {
                var snapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber>();
                snapshot.AppendSections(new NSNumber[] { 1 });
                snapshot.AppendItems(SafeViewModel.Items.Select(it => (NSNumber)it.Id).ToArray(), 1);
                DataSource.ApplySnapshot(snapshot, true);
            })
            .DisposeWith(disposables);
    }

    protected override UIView CreateView()
    {
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

        Feed = new UICollectionView(CGRect.Empty, layout) { Delegate = this };

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
                NumComments = tItem.NumComments
            };
            cell.ContentConfiguration = contentConfig;

            var bgConfig = UIBackgroundConfiguration.ListPlainCellConfiguration;
            bgConfig.BackgroundColorTransformer = color =>
            {
                var state = cell.ConfigurationState;
                if (state.Highlighted || state.Selected)
                {
                    return UIColor.SystemGray3;
                }

                return UIColor.Clear;
            };
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

internal class StoryFeedView : UIView, IUIContentView
{
    private readonly NoctalLabel LblArticleNumber;
    private readonly NoctalLabel LblAuthor;
    private readonly NoctalLabel LblNumComments;
    private readonly NoctalLabel LblScore;
    private readonly NoctalLabel LblTimeAgo;
    private readonly NoctalLabel LblTitle;
    private readonly NoctalLabel LblUrl;

    private IUIContentConfiguration _configuration;

    private StoryFeedView(StoryFeedConfiguration configuration) : base(CGRect.Empty)
    {
        _configuration = configuration;

        var makeLabel = () => new NoctalLabel { TranslatesAutoresizingMaskIntoConstraints = false };

        var img = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform() };
        img.Layer.CornerRadius = (nfloat)StoriesPage.Dims.DimImgRadius;
        AddSubview(img);

        var sv = new UIStackView { TranslatesAutoresizingMaskIntoConstraints = false, Axis = UILayoutConstraintAxis.Vertical, Alignment = UIStackViewAlignment.Leading, Spacing = (nfloat)StoriesPage.Dims.DimVPadding };
        AddSubview(sv);

        // Url Row

        var row = new UIStackView { TranslatesAutoresizingMaskIntoConstraints = false, Axis = UILayoutConstraintAxis.Horizontal, Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow };

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

        row = new UIStackView { TranslatesAutoresizingMaskIntoConstraints = false, Axis = UILayoutConstraintAxis.Horizontal, Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow };

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

        row = new UIStackView { TranslatesAutoresizingMaskIntoConstraints = false, Axis = UILayoutConstraintAxis.Horizontal, Spacing = (nfloat)StoriesPage.Dims.DimHPaddingRow };

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

        var separator = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.OpaqueSeparator };
        AddSubview(separator);

        NSLayoutConstraint.ActivateConstraints(new[] { separator.HeightAnchor.ConstraintEqualTo(1), separator.LeadingAnchor.ConstraintEqualTo(LeadingAnchor), separator.TrailingAnchor.ConstraintEqualTo(TrailingAnchor), separator.BottomAnchor.ConstraintEqualTo(BottomAnchor), img.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, (nfloat)StoriesPage.Dims.DimHPadding), img.CenterYAnchor.ConstraintEqualTo(CenterYAnchor), img.WidthAnchor.ConstraintEqualTo((nfloat)StoriesPage.Dims.DimImg), img.HeightAnchor.ConstraintEqualTo(img.WidthAnchor), sv.LeadingAnchor.ConstraintEqualTo(img.TrailingAnchor, (nfloat)StoriesPage.Dims.DimHPadding), sv.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -(nfloat)StoriesPage.Dims.DimHPadding), sv.TopAnchor.ConstraintEqualTo(TopAnchor, (nfloat)StoriesPage.Dims.DimVPadding), sv.BottomAnchor.ConstraintEqualTo(separator.TopAnchor, -(nfloat)StoriesPage.Dims.DimVPadding) });

        updateConfiguration(configuration);
    }

    public IUIContentConfiguration Configuration
    {
        get => _configuration;
        set
        {
            _configuration = value;
            updateConfiguration(value);
        }
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
                NumComments = NumComments
            };
        }

        public IUIContentConfiguration GetUpdatedConfiguration(IUIConfigurationState state)
        {
            return this;
        }

        public IUIContentView MakeContentView()
        {
            return new StoryFeedView(this);
        }
    }
}

internal class ObjectWrapper<T> : NSObject where T : class
{
    public ObjectWrapper(T value)
    {
        Value = value;
    }

    public T Value { get; }
}
#endif
