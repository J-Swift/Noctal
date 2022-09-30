#if ANDROID
using Color = Microsoft.Maui.Graphics.Color;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Shape;
using Google.Android.Material.ImageView;
using Orientation = Android.Widget.Orientation;
#elif IOS
using CoreGraphics;
using Foundation;
using Noctal.UI.Theming;
using ObjCRuntime;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
#endif
using DynamicData.Binding;
using Noctal.ImageLoading;
using Noctal.Stories.Models;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
        public static readonly double DimImgFavicon = 16;
        public static readonly double DimImgRadius = 4;
        public static readonly double DimVPadding = 16;
        public static readonly double DimHPaddingRow = 4;
        public static readonly double DimHPadding = 20;

        public static readonly double DimEstimatedCellHeight = 160;
    }

    public static class Styling
    {
        public static readonly Color CellHighlightLt = Color.FromRgb(180, 215, 250);
        public static readonly Color CellHighlightDk = Color.FromRgb(28, 104, 185);
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
    private static readonly int BaseId = 839183;
    private static readonly int LblArticleNumberId = BaseId++;
    private static readonly int LblUrlId = BaseId++;
    private static readonly int LblTitleId = BaseId++;
    private static readonly int LblAuthorId = BaseId++;
    private static readonly int LblTimeAgoId = BaseId++;
    private static readonly int LblScoreId = BaseId++;
    private static readonly int LblNumCommentsId = BaseId++;
    private static readonly int ImgFaviconId = BaseId++;
    private static readonly int ImgImageId = BaseId++;
    private IList<StoriesFeedItem> Items;
    public EventHandler<EventArgs>? ItemSelected;

    public MyAdapter(IList<StoriesFeedItem> items)
    {
        Items = items;
    }

    public override int ItemCount => Items.Count;

    public void SetItems(IList<StoriesFeedItem> newItems)
    {
        var cb = new Callback(Items, newItems);
        var diff = DiffUtil.CalculateDiff(cb);
        Items = newItems;
        diff.DispatchUpdatesTo(this);
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var dimImg = parent.Context.ToPixels(StoriesPage.Dims.DimImg);
        var view = CreateView(parent.Context!);
        return new ViewHolder(view, dimImg);
    }

    private View CreateView(Context context)
    {
        var dimImg = context.ToPixels(StoriesPage.Dims.DimImg);
        var dimImgFavicon = context.ToPixels(StoriesPage.Dims.DimImgFavicon);
        var dimImgRadius = context.ToPixels(StoriesPage.Dims.DimImgRadius);
        var dimVPadding = context.ToPixels(StoriesPage.Dims.DimVPadding);
        var dimHPadding = context.ToPixels(StoriesPage.Dims.DimHPadding);
        var dimHPaddingRow = context.ToPixels(StoriesPage.Dims.DimHPaddingRow);
        var dimVSpacerPs = new LinearLayout.LayoutParams(1, (int)dimVPadding);
        var dimHSpacerPs = new LinearLayout.LayoutParams((int)dimHPaddingRow, 1);
        var parent = ConstraintLayout.LayoutParams.ParentId;

        var isNight = AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightYes;

        var csl = new ColorStateList(new[]
            {
                new int[]
                {
                    Android.Resource.Attribute.StatePressed,
                },
                Array.Empty<int>(),
            },
            new int[]
            {
                (isNight ? StoriesPage.Styling.CellHighlightDk : StoriesPage.Styling.CellHighlightLt).ToPlatform(),
                Android.Resource.Color.Transparent,
            });

        var container = new ConstraintLayout(context);
        container.Background = new ColorStateListDrawable(csl);

        var shapeModel = new ShapeAppearanceModel().ToBuilder()
            .SetAllCorners(CornerFamily.Rounded, dimImgRadius)
            .Build();

        var img = new ShapeableImageView(context)
        {
            Id = ImgImageId,
            ShapeAppearanceModel = shapeModel,
        };
        img.SetScaleType(ImageView.ScaleType.CenterCrop);

        container.AddView(img);

        var sv = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
            Id = 18310,
        };
        container.AddView(sv);

        // Url Row

        var row = new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
        };
        row.SetVerticalGravity(GravityFlags.Center);

        var tv = new NoctalLabel(context)
        {
            Id = LblArticleNumberId,
        };
        row.AddView(tv);

        var spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        shapeModel = new ShapeAppearanceModel().ToBuilder()
            .SetAllCornerSizes(new RelativeCornerSize(0.5f))
            .Build();

        var imgFavicon = new ShapeableImageView(context)
        {
            Id = ImgFaviconId,
            ShapeAppearanceModel = shapeModel,
        };
        row.AddView(imgFavicon, new ViewGroup.LayoutParams((int)dimImgFavicon, (int)dimImgFavicon));

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context)
        {
            Id = LblUrlId,
        };
        tv.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
        tv.SetMaxLines(1);
        row.AddView(tv);

        sv.AddView(row);

        // Title Row

        tv = new NoctalLabel(context)
        {
            Id = LblTitleId,
        };

        spacer = new View(context);
        sv.AddView(spacer, dimVSpacerPs);
        sv.AddView(tv);

        // Author Row

        row = new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
        };

        tv = new NoctalLabel(context)
        {
            Id = LblAuthorId,
        };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context)
        {
            Text = "•",
        };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context)
        {
            Id = LblTimeAgoId,
            Text = "•",
        };
        row.AddView(tv);

        spacer = new View(context);
        sv.AddView(spacer, dimVSpacerPs);
        sv.AddView(row);

        // Score Row

        row = new LinearLayout(context)
        {
            Orientation = Orientation.Horizontal,
        };

        tv = new NoctalLabel(context)
        {
            Id = LblScoreId,
        };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context)
        {
            Text = "•",
        };
        row.AddView(tv);

        spacer = new View(context);
        row.AddView(spacer, dimHSpacerPs);

        tv = new NoctalLabel(context)
        {
            Id = LblNumCommentsId,
            Text = "•",
        };
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
        private static readonly IList<Color> BgColors = new[]
        {
            Color.FromRgb(149, 176, 214),
            Color.FromRgb(143, 144, 161),
            Color.FromRgb(81, 91, 92),
            Color.FromRgb(52, 63, 62),
        };

        private readonly View Container;
        private readonly float DimImg;
        private readonly IImageLoader ImageLoader;
        private readonly ShapeableImageView ImgFavicon;
        private readonly ShapeableImageView ImgImage;
        private readonly NoctalLabel LblArticleNumber;
        private readonly NoctalLabel LblAuthor;
        private readonly NoctalLabel LblNumComments;
        private readonly NoctalLabel LblScore;
        private readonly NoctalLabel LblTimeAgo;
        private readonly NoctalLabel LblTitle;
        private readonly NoctalLabel LblUrl;
        private Action? OnClick;

        public ViewHolder(View view, float dimImg) : base(view)
        {
            Container = view;
            DimImg = dimImg;
            ImgFavicon = view.FindViewById<ShapeableImageView>(ImgFaviconId)!;
            ImgImage = view.FindViewById<ShapeableImageView>(ImgImageId)!;
            LblArticleNumber = view.FindViewById<NoctalLabel>(LblArticleNumberId)!;
            LblUrl = view.FindViewById<NoctalLabel>(LblUrlId)!;
            LblTitle = view.FindViewById<NoctalLabel>(LblTitleId)!;
            LblAuthor = view.FindViewById<NoctalLabel>(LblAuthorId)!;
            LblTimeAgo = view.FindViewById<NoctalLabel>(LblTimeAgoId)!;
            LblScore = view.FindViewById<NoctalLabel>(LblScoreId)!;
            LblNumComments = view.FindViewById<NoctalLabel>(LblNumCommentsId)!;
            ImageLoader = ServiceProvider.GetService<IImageLoader>();
        }

        public void Bind(int articleNumber, StoriesFeedItem model, Action onClick)
        {
            LblArticleNumber.Text = $"{articleNumber}.";
            LblUrl.Text = model.DisplayUrl ?? " ";
            LblTitle.Text = model.Title;
            LblAuthor.Text = model.Submitter;
            LblTimeAgo.Text = model.TimeAgo;
            LblScore.Text = model.Score.ToString();
            LblNumComments.Text = $"{model.NumComments} comment" + (model.NumComments > 1 ? "s" : "");
            ImageLoader.LoadInto(Container.Context!, new IImageLoader.LoadRequest(ImgImage, model.ImagePath, PlaceholderFor(articleNumber, model.PlaceholderLetter ?? "Y")));
            ImageLoader.LoadInto(Container.Context!, new IImageLoader.LoadRequest(ImgFavicon, model.FavIconPath));

            OnClick = onClick;
            Container.Click -= HandleClick;
            Container.Click += HandleClick;
        }

        // TODO(jpr): cache
        private Drawable PlaceholderFor(int articleNumber, string letter)
        {
            var view = new NoctalLabel(Container.Context);
            view.SetBackgroundColor(BgColors[articleNumber % BgColors.Count].ToPlatform());
            view.SetTextColor(Colors.White.ToPlatform());
            view.Text = letter.ToUpper();
            view.TextSize = 32;
            view.Gravity = GravityFlags.Center;
            view.LayoutParameters = new ViewGroup.LayoutParams((int)DimImg, (int)DimImg);
            var spec = View.MeasureSpec.MakeMeasureSpec((int)DimImg, MeasureSpecMode.Exactly);
            view.Measure(spec, spec);
            view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);

            var bitmap = Bitmap.CreateBitmap(
                view.Width,
                view.Height,
                Bitmap.Config.Argb8888!
            )!;
            var canvas = new Canvas(bitmap);
            view.Draw(canvas);
            return new BitmapDrawable(Container.Context!.Resources, bitmap);
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
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(it =>
            {
                var action = it.EventArgs.Action;
                if (action == NotifyCollectionChangedAction.Replace)
                {
                    var item = SafeViewModel.Items[it.EventArgs.NewStartingIndex];
                    if (item is null)
                    {
                        Console.WriteLine($"Item not found [{item?.Id}]");
                        return;
                    }

                    var snapshot = DataSource.Snapshot;
                    snapshot.ReconfigureItems(new NSNumber[]
                    {
                        item.Id,
                    });
                    DataSource.ApplySnapshot(snapshot, true);
                }
                else
                {
                    var items = SafeViewModel.Items;
                    var snapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber>();
                    snapshot.AppendSections(new NSNumber[]
                    {
                        1,
                    });
                    snapshot.AppendItems(items.Select(it => (NSNumber)it.Id).ToArray(), 1);
                    DataSource.ApplySnapshot(snapshot, true);
                }
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

        Feed = new UICollectionView(CGRect.Empty, layout)
        {
            Delegate = this,
        };

        var cellReg = UICollectionViewCellRegistration.GetRegistration(typeof(UICollectionViewCell),
            (cell, indexPath, item) =>
            {
                var tItem = ((ObjectWrapper<StoriesFeedItem>)item).Value;
                var contentConfig = new StoryFeedView.StoryFeedConfiguration
                {
                    DimImg = (int)Dims.DimImg,
                    ItemNumber = indexPath.Row + 1,
                    Url = tItem.Url,
                    DisplayUrl = tItem.DisplayUrl,
                    PlaceholderLetter = tItem.PlaceholderLetter,
                    Title = tItem.Title,
                    Submitter = tItem.Submitter,
                    TimeAgo = tItem.TimeAgo,
                    Score = tItem.Score,
                    NumComments = tItem.NumComments,
                    ImagePath = tItem.ImagePath,
                    FavIconPath = tItem.FavIconPath,
                };
                cell.ContentConfiguration = contentConfig;

                var bgConfig = UIBackgroundConfiguration.ListPlainCellConfiguration;
                bgConfig.BackgroundColorTransformer = color =>
                {
                    var state = cell.ConfigurationState;
                    if (state.Highlighted || state.Selected)
                    {
                        return ColorUtils.Adaptive(Styling.CellHighlightLt, Styling.CellHighlightDk);
                    }

                    return UIColor.Clear;
                };
                cell.BackgroundConfiguration = bgConfig;
            });

        DataSource = new UICollectionViewDiffableDataSource<NSNumber, NSNumber>(Feed,
            (collectionView, indexPath, itemIdentifier) =>
            {
                var item = SafeViewModel.GetItem(((NSNumber)itemIdentifier).Int32Value);
                return collectionView.DequeueConfiguredReusableCell(cellReg, indexPath, new ObjectWrapper<StoriesFeedItem>(item));
            });

        Feed.RegisterClassForCell(typeof(UICollectionViewCell), "cell");

        return Feed;
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

internal sealed class StoryFeedView : UIView, IUIContentView
{
    private static readonly IList<Color> BgColors = new[]
    {
        Color.FromRgb(149, 176, 214),
        Color.FromRgb(143, 144, 161),
        Color.FromRgb(81, 91, 92),
        Color.FromRgb(52, 63, 62),
    };

    private readonly IImageLoader ImageLoader;
    private readonly UIImageView ImgFavicon;
    private readonly UIImageView ImgImage;
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
        ImageLoader = ServiceProvider.GetService<IImageLoader>();
        _configuration = configuration;

        var makeLabel = () => new NoctalLabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        // ImgImage = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false, ClipsToBounds = true, ContentMode = UIViewContentMode.ScaleAspectFill, BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform() };
        ImgImage = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill,
        };
        ImgImage.Layer.CornerRadius = (nfloat)StoriesPage.Dims.DimImgRadius;
        AddSubview(ImgImage);

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

        ImgFavicon = new UIImageView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            ClipsToBounds = true,
            ContentMode = UIViewContentMode.ScaleAspectFill,
            BackgroundColor = Colors.Red.WithAlpha(0.3f).ToPlatform(),
        };
        ImgFavicon.HeightAnchor.ConstraintEqualTo((NFloat)StoriesPage.Dims.DimImgFavicon).Active = true;
        ImgFavicon.WidthAnchor.ConstraintEqualTo(ImgFavicon.HeightAnchor).Active = true;
        ImgFavicon.Layer.CornerRadius = (NFloat)(StoriesPage.Dims.DimImgFavicon / 2.0);
        row.AddArrangedSubview(ImgFavicon);

        lbl = makeLabel();
        lbl.AdjustsFontSizeToFitWidth = true;
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

            ImgImage.LeadingAnchor.ConstraintEqualTo(LeadingAnchor,
                (nfloat)StoriesPage.Dims.DimHPadding),
            ImgImage.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
            ImgImage.WidthAnchor.ConstraintEqualTo((nfloat)StoriesPage.Dims.DimImg),
            ImgImage.HeightAnchor.ConstraintEqualTo(ImgImage.WidthAnchor),

            sv.LeadingAnchor.ConstraintEqualTo(ImgImage.TrailingAnchor, (nfloat)StoriesPage.Dims.DimHPadding),
            sv.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -(nfloat)StoriesPage.Dims.DimHPadding),
            sv.TopAnchor.ConstraintEqualTo(TopAnchor, (nfloat)StoriesPage.Dims.DimVPadding),
            sv.BottomAnchor.ConstraintEqualTo(separator.TopAnchor, -(nfloat)StoriesPage.Dims.DimVPadding),
        });

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
            LblUrl.Text = config.DisplayUrl ?? " ";
            LblTitle.Text = config.Title;
            LblAuthor.Text = config.Submitter;
            LblTimeAgo.Text = config.TimeAgo;
            LblScore.Text = config.Score.ToString();
            LblNumComments.Text = $"{config.NumComments} comment" + (config.NumComments > 1 ? "s" : "");
            ImageLoader.LoadInto(new IImageLoader.LoadRequest(ImgImage, config.ImagePath, PlaceholderFor(config.DimImg, config.ItemNumber, config.PlaceholderLetter ?? "Y")));
            ImageLoader.LoadInto(new IImageLoader.LoadRequest(ImgFavicon, config.FavIconPath));
        }
    }

    // TODO(jpr): cache
    private UIImage PlaceholderFor(int dimImg, int articleNumber, string letter)
    {
        var view = new NoctalLabel();
        view.BackgroundColor = BgColors[articleNumber % BgColors.Count].ToPlatform();
        view.TextColor = Colors.White.ToPlatform();
        view.Text = letter.ToUpper();
        view.Font = view.Font.WithSize(32);
        view.TextAlignment = UITextAlignment.Center;
        view.Frame = new CGRect(0, 0, dimImg, dimImg);

        var renderer = new UIGraphicsImageRenderer(view.Bounds.Size);
        var image = renderer.CreateImage(ctx =>
        {
            view.Layer.RenderInContext(ctx.CGContext);
        });
        return image;
    }

    public class StoryFeedConfiguration : NSObject, IUIContentConfiguration
    {
        public int ItemNumber { get; set; } = 1;
        public string Url { get; set; } = "";
        public string? DisplayUrl { get; set; }
        public string? PlaceholderLetter { get; set; }
        public string Title { get; set; } = "";
        public string Submitter { get; set; } = "";
        public string TimeAgo { get; set; } = "";
        public int Score { get; set; }
        public int NumComments { get; set; }
        public string? FavIconPath { get; set; }
        public string? ImagePath { get; set; }
        public int DimImg { get; set; }

        [return: Release]
        public NSObject Copy(NSZone? zone)
        {
            return new StoryFeedConfiguration
            {
                ItemNumber = ItemNumber,
                Url = Url,
                DisplayUrl = DisplayUrl,
                PlaceholderLetter = PlaceholderLetter,
                Title = Title,
                Submitter = Submitter,
                TimeAgo = TimeAgo,
                Score = Score,
                NumComments = NumComments,
                FavIconPath = FavIconPath,
                ImagePath = ImagePath,
                DimImg = DimImg,
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
