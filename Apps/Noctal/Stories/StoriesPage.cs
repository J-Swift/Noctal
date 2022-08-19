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
using UIKit;
#endif

namespace Noctal.Stories;

public partial class StoriesPage : BasePage<StoriesViewModel>
{
    public record EventArgs(StoriesFeedItem SelectedItem);
    public EventHandler<EventArgs>? OnItemSelected;

    protected override StoriesViewModel CreateViewModel() => new(new StoriesService());
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
        var dimImg = context.ToPixels(70);
        var dimVPadding = context.ToPixels(16);
        var dimHPadding = context.ToPixels(20);
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
public partial class StoriesPage : IUICollectionViewDataSource, IUICollectionViewDelegate
{
    private UICollectionView Feed { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
    }

    protected override UIView CreateView()
    {
        var view = new UIView();

        var itemSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated(160)
        );
        var item = NSCollectionLayoutItem.Create(itemSize);

        var groupSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated(160)
        );
        var group = NSCollectionLayoutGroup.CreateHorizontal(groupSize, item);

        var section = NSCollectionLayoutSection.Create(group);
        var layout = new UICollectionViewCompositionalLayout(section);

        Feed = new UICollectionView(CGRect.Empty, layout)
        {
            DataSource = this,
            Delegate = this,
        };
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

    [Export("numberOfSectionsInCollectionView:")]
    public nint NumberOfSections(UICollectionView collectionView)
    {
        return 1;
    }

    [Export("collectionView:numberOfItemsInSection:")]
    public nint GetItemsCount(UICollectionView collectionView, nint section)
    {
        return SafeViewModel.Items.Count;
    }

    [Export("collectionView:cellForItemAtIndexPath:")]
    public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var item = SafeViewModel.Items[indexPath.Row];
        var cell = (UICollectionViewCell)collectionView.DequeueReusableCell("cell", indexPath);

        var contentConfig = UIListContentConfiguration.ValueCellConfiguration;
        contentConfig.Text = item.Title;
        cell.ContentConfiguration = contentConfig;

        var bgConfig = UIBackgroundConfiguration.ListPlainCellConfiguration;
        cell.BackgroundConfiguration = bgConfig;

        return cell;
    }

    [Export("collectionView:didSelectItemAtIndexPath:")]
    public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var item = SafeViewModel.Items[indexPath.Row]!;
        OnItemSelected?.Invoke(this, new EventArgs(item!));
    }
}
#endif
